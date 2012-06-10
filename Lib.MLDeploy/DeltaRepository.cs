using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lib.MLDeploy
{
    internal class DeltaRepository : IDeltaRepository
    {
        private readonly string _deltasDir;

        public DeltaRepository(string deltasDir)
        {
            _deltasDir = deltasDir;
        }

        public List<Delta> GetAllDeltas()
        {
            var filesInDir = Directory.GetFiles(_deltasDir, "*.xqy");
            return filesInDir.Select(CreateDeltaFromFileName).ToList();
        }

        private Delta CreateDeltaFromFileName(string fullFilePath)
        {
            string[] filePathSplit = fullFilePath.Split('\\');
            var fileName = filePathSplit[filePathSplit.Length - 1];
            long number = new DeltaFileName(fileName).ParseDeltaNumber();
            return new Delta(number, fullFilePath);
        }

        internal class DeltaFileName
        {
            private readonly string _fileName;

            internal DeltaFileName(string fileName)
            {
                _fileName = fileName;
            }

            internal long ParseDeltaNumber()
            {
                var fileNameWithoutExtension = _fileName.Split('.')[0];

                var numberString = GetNumberString(fileNameWithoutExtension);

                if(numberString.Length == 0)
                    throw new ArgumentException(string.Format("Oops!! There seems to be an xquery file which doesn't start with a number called {0}.xqy", fileNameWithoutExtension));
                return ParseNumberString(numberString);
            }

            private long ParseNumberString(string numberString)
            {
                var reverseNumberString = numberString.Reverse();
                var modulo = 1;
                long number = 0;

                foreach (var num in reverseNumberString.Select(character => Int64.Parse(new string(new[] {character}))))
                {
                    number += num * modulo;
                    modulo *= 10;
                }

                return number;
            }

            private string GetNumberString(IEnumerable<char> name)
            {
                var numberString = new StringBuilder();
                
                foreach (char character in name)
                {
                    long val;
                    if(Int64.TryParse(new string(new[]{character}), out val))
                        numberString.Append(val);
                    else
                        break;
                }
                return numberString.ToString();
            }
        }
    }
}