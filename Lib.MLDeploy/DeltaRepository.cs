using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Marklogic.Xcc;
using UnitTests.MLDeploy;

namespace Lib.MLDeploy
{
    internal class DeltaRepository:IDeltaRepository
    {
        private readonly string _path;
        private readonly string _connectionString;

        public DeltaRepository(string connectionString, string path)
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
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));


            using (var session = contentSource.NewSession())
            {
                const string xqueryToExcecute = @"xquery version ""1.0-ml"";
                                           declare namespace m=""http://mldeploy.org"";
                                           for $doc in //*:LatestDelta 
                                           return $doc/m:Number/text()";

                Request request = session.NewAdhocQuery(xqueryToExcecute);
                string result = session.SubmitRequest(request).AsString();

                if(result == string.Empty)
                {
                    return new NoDelta();
                }

                long number = Int64.Parse(result);
                return new Delta(number, string.Format("{0}\\{1}.xqy", _path, number));
            }
        }

        public void ApplyDelta(Delta delta)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));

            Console.WriteLine("[mldeploy] Applying delta "+ delta.Number);

            using (var session = contentSource.NewSession())
            {
                string xqueryToExcecute = File.ReadAllText(delta.Path);
                Request request = session.NewAdhocQuery(xqueryToExcecute);
                session.SubmitRequest(request).AsString();
            }
        }

        public void UpdateLatestDeltaAs(Delta delta)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));

            using (var session = contentSource.NewSession())
            {
                string xqueryToExcecute =
                    string.Format(
                        @"xdmp:document-insert(""/mldeploy/latest.xml"", <LatestDelta xmlns:m=""http://mldeploy.org""><m:Number>{0}</m:Number></LatestDelta>, ())", delta.Number);
                Request request = session.NewAdhocQuery(xqueryToExcecute);
                session.SubmitRequest(request).AsString();
            }
        }
    }
}