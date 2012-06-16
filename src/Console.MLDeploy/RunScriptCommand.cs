using System;
using Lib.MLDeploy;
using ManyConsole;

namespace Console.MLDeploy
{
    internal class RunScriptCommand : ConsoleCommand
    {
        private const string CONN_STR_MESSAGE = "xcc connection string to the Marklogic XDBC server : For example xcc://username:password@127.0.0.1:8080";
        private const string XQUERY_FILE_MESSAGE = "path to the xquery script";
        private const string COMMAND_DESCRIPTION = "runs an xquery script from a file";

        private string _scriptPath;
        private string _connectionString;


        public RunScriptCommand()
        {
            this.IsCommand("runscript", COMMAND_DESCRIPTION);
            this.HasRequiredOption("connstring|c:", CONN_STR_MESSAGE, cs => _connectionString = cs);
            this.HasRequiredOption("scriptpath|s:", XQUERY_FILE_MESSAGE, p => _scriptPath = p);
        }

        public override int Run(string[] remainingArguments)
        {
            new ScriptRunner(_connectionString, _scriptPath).Run();
            return 0;
        }
    }

    
}