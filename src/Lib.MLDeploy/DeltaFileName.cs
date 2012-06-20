using System;
using System.Linq;
using System.Text;

namespace Lib.MLDeploy
{
    internal class DeltaFileName
    {
        private readonly string _fileName;
        private readonly long _deltaNumber;

        internal DeltaFileName(string fullFileName)
        {
            string[] filePathSplit = fullFileName.Split('\\');
            _fileName = filePathSplit[filePathSplit.Length - 1];
            _deltaNumber = ParseNumber();
        }

        internal long Number
        {
            get { return _deltaNumber; }
        }

       

        internal string Description()
        {
            var extensionlessName = _fileName.Split('.')[0];
            var split = extensionlessName.Split(new[] { _deltaNumber.ToString() }, StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 0)
                return string.Empty;

            return extensionlessName.Substring(extensionlessName.IndexOf(split[0]) + split.Length - 1)
                                    .Trim();

        }

        private long ParseNumber()
        {
            var extensionlessName = _fileName.Split('.')[0];
            var numberString = new StringBuilder();

            foreach (var character in extensionlessName.TakeWhile(character => character.IsLong()))
            {
                numberString.Append(character.ToLong());
            }

            if (numberString.Length == 0)
                throw new ArgumentException(string.Format("Oops!! There seems to be an xquery file which doesn't start with a number called {0}.xqy", extensionlessName));
    
            return numberString.ToString().ToLong();
        }
    }
}