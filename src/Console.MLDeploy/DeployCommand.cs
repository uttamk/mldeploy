using Lib.MLDeploy;
using ManyConsole;

namespace Console.MLDeploy
{
    internal class DeployCommand : ConsoleCommand
    {
        private const string CONN_STR_MESSAGE = "xcc connection string to the Marklogic XDBC server : For example xcc://username:password@127.0.0.1:8080";
        private const string DELTAS_PATH_MESSAGE = "directory path where the deltas are stored";
        private const string COMMAND_DESCRIPTION = "deploys delta xquery scripts from the specified deltas directory";

        private string _deltasPath;
        private string _connectionString;


        internal DeployCommand()
        {
            this.IsCommand("deploy", COMMAND_DESCRIPTION);
            this.HasRequiredOption("connstring|c:", CONN_STR_MESSAGE, cs => _connectionString = cs);
            this.HasRequiredOption("deltaspath|d:", DELTAS_PATH_MESSAGE, p => _deltasPath = p);
        }

        public override int Run(string[] remainingArguments)
        {
            new Deployer(_connectionString, _deltasPath).Deploy();
            return 0;
        }
    }
}