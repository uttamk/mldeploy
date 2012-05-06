using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Marklogic.Xcc;

namespace Lib.MLDeploy
{
    internal class DeltaRepository:IDeltaRepository
    {
        private readonly string _path;
        private readonly string _connectionString;

        public DeltaRepository(string path, string connectionString)
        {
            _path = path;
            _connectionString = connectionString;
        }

        public List<Delta> GetAllDeltas()
        {
            var filesInDir = Directory.GetFiles(_path, "*.xqy");
            return filesInDir.Select(CreateDeltaFromFileName).ToList();

        }

        private Delta CreateDeltaFromFileName(string fullFilePath)
        {
            string[] filePathSplit = fullFilePath.Split('\\');
            var fileName = filePathSplit[filePathSplit.Length - 1];
            long number = Int64.Parse(fileName.Split('.')[0]);
            return new Delta(number, fullFilePath);
        }

        public Delta GetLatestDeltaInDatabase()
        {
            throw new NotImplementedException();
        }

        public void ApplyDelta(Delta delta)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));

            Console.WriteLine("[mldeploy] Applying delta "+ delta.Number);

            using (var session = contentSource.NewSession())
            {
                Request request = session.NewAdhocQuery(File.ReadAllText(delta.Path));
                session.SubmitRequest(request).AsString();
            }
        }

        public void UpdateLatestDeltaAs(Delta delta)
        {
            throw new NotImplementedException();
        }
    }
}