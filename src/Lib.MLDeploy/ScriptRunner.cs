using System;
using System.IO;
using Marklogic.Xcc;

namespace Lib.MLDeploy
{
    internal class ScriptRunner
    {
        private readonly string _connectionString;
        private readonly string _scriptPath;

        public ScriptRunner(string connectionString, string scriptPath)
        {
            _connectionString = connectionString;
            _scriptPath = scriptPath;
        }

        public void Run()
        {
            Console.WriteLine("Executing xquery script at " + _scriptPath);
            var script = File.ReadAllText(_scriptPath);
            Run(script);
        }

        private void Run(string script)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));
            using (var session = contentSource.NewSession())
            {
              
                Request request = session.NewAdhocQuery(script);
                session.SubmitRequest(request).AsString();

            }
        }
    }
}