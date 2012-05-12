using ManyConsole;

namespace Console.MLDeploy
{
    class RollBackScriptCommand : ConsoleCommand
    {
        private const string COMMAND_DESCRIPTION = "Generates rollback xquery scripts from the specified deltas directory";
        private const string DELTA_PATH_DESCRIPTION = "The path where the deltas reside";
        private const string OUTPUT_PATH_DESCRIPTION = "Output path for the rollback script";
        private const string FROM_DELTA_DESCRIPTION = "The delta to start the rollback from : starts from 0";
        private const string TO_DELTA_DESCRIPTION = "The last delta in the rollback";

        private string _deltasPath;
        private string _outputpath;
        private string _fromDelta;
        private string _toDelta;

        public RollBackScriptCommand()
        {
            this.IsCommand("rbscript", COMMAND_DESCRIPTION);
            this.HasRequiredOption("deltaspath|d:", DELTA_PATH_DESCRIPTION, d => _deltasPath = d);
            this.HasRequiredOption("outputpath|o:", OUTPUT_PATH_DESCRIPTION, o => _outputpath = o);
            this.HasOption("fromdelta|f:", FROM_DELTA_DESCRIPTION, f => _fromDelta = f);
            this.HasOption("todelta|t:", TO_DELTA_DESCRIPTION, t => _toDelta = t);
        }
        
        public override int Run(string[] remainingArguments)
        {
            return 0;
        }
    }
}