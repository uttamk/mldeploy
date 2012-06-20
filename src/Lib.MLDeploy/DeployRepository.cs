using System;
using System.IO;
using Marklogic.Xcc;

namespace Lib.MLDeploy
{
    internal class DeployRepository:IDeployRepository
    {
        private readonly string _path;
        private readonly string _connectionString;

        internal DeployRepository(string connectionString, string path)
        {
            _path = path;
            _connectionString = connectionString;
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

                var number = Int64.Parse(result);
                var description = GetLatestDeltaDescriptionFromDatabase();
                return new Delta(number, string.Format("{0}\\{1}.xqy", _path, number), description);
            }
        }

        private string GetLatestDeltaDescriptionFromDatabase()
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));


            using (var session = contentSource.NewSession())
            {
                const string xqueryToExcecute = @"xquery version ""1.0-ml"";
                                           declare namespace m=""http://mldeploy.org"";
                                           for $doc in //*:LatestDelta 
                                           return $doc/m:Description/text()";

                Request request = session.NewAdhocQuery(xqueryToExcecute);
                string result = session.SubmitRequest(request).AsString();
                return result;
            }
        }

        public void ApplyDelta(Delta delta)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));

            Console.WriteLine(string.Format("[mldeploy] Applying delta {0}  ({1})", delta.Number, delta.Description));
            

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
                        @"xdmp:document-insert(""/mldeploy/latest.xml"", <LatestDelta xmlns:m=""http://mldeploy.org""><m:Number>{0}</m:Number><m:Description>{1}</m:Description></LatestDelta>, ())", delta.Number, delta.Description);
                Request request = session.NewAdhocQuery(xqueryToExcecute);
                session.SubmitRequest(request).AsString();
            }
        }
    }
}