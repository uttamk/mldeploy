using System;
using System.Linq;
using System.Text;

namespace Lib.MLDeploy
{
    internal class DeltaFileName
    {
        private readonly string _fileName;
        private readonly string _numberString;

        internal DeltaFileName(string fileName)
        {
            _fileName = fileName;
            _numberString = GetNumberString();
        }

        internal long DeltaNumber()
        {
            var reverseNumberString = _numberString.Reverse();
            var modulo = 1;
            long number = 0;

            foreach (var num in reverseNumberString.Select(character => Int64.Parse(new string(new[] { character }))))
            {
                number += num * modulo;
                modulo *= 10;
            }

            return number;
        }

        internal string Description()
        {
            var extensionlessName = _fileName.Split('.')[0];

            var split = extensionlessName.Split(new[] { _numberString }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 0)
                return string.Empty;

            var descriptionString = split[0];

            return descriptionString.Trim();

        }

        private string GetNumberString()
        {
            var extensionlessName = _fileName.Split('.')[0];
            var numberString = new StringBuilder();

            foreach (char character in extensionlessName)
            {
                long val;
                if (Int64.TryParse(new string(new[] { character }), out val))
                    numberString.Append(val);
                else
                    break;
            }

            if (numberString.Length == 0)
                throw new ArgumentException(string.Format("Oops!! There seems to be an xquery file which doesn't start with a number called {0}.xqy", extensionlessName));
    
            return numberString.ToString();
        }
    }
}