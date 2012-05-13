using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            long number = Int64.Parse(fileName.Split('.')[0]);
            return new Delta(number, fullFilePath);
        }
    }
}