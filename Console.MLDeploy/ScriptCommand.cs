using Lib.MLDeploy;
using ManyConsole;

namespace Console.MLDeploy
{
    internal class ScriptCommand : ConsoleCommand
    {
        private const string COMMAND_DESCRIPTION = "Generates xquery scripts from the specified deltas directory";

        private const string DELTA_PATH_DESCRIPTION = "The path where the deltas reside";
        private const string OUTPUT_PATH_DESCRIPTION = "Output path for the script";
        private const string FROM_DELTA_DESCRIPTION = "The delta to start the script";
        private const string TO_DELTA_DESCRIPTION = "The last delta in the script";
        private const string ROLLBACK_DESCRIPTION = "If the script type is rollback";

        private string _deltasPath;
        private string _outputpath;
        private string _fromDelta;
        private string _toDelta;
        private bool _isRollback;

        public ScriptCommand()
        {
            this.IsCommand("script", COMMAND_DESCRIPTION);
            this.HasRequiredOption("deltaspath|d:", DELTA_PATH_DESCRIPTION, d => _deltasPath = d);
            this.HasRequiredOption("outputpath|o:", OUTPUT_PATH_DESCRIPTION, o => _outputpath = o);
            this.HasOption("startdelta|s:", FROM_DELTA_DESCRIPTION, f => _fromDelta = f);
            this.HasOption("enddelta|e:", TO_DELTA_DESCRIPTION, t => _toDelta = t);
            this.HasOption("rollback|r", ROLLBACK_DESCRIPTION, r => _isRollback = r != null);
        }

        public override int Run(string[] remainingArguments)
        {
            if (_isRollback)
            {
                new Script(new ScriptRepository(_outputpath), new DeltaRepository(_deltasPath))
                    .StartingFrom(_fromDelta)
                    .EndingWith(_toDelta)
                    .GenerateDeploy();
            }
            else
            {
                new Script(new ScriptRepository(_outputpath), new DeltaRepository(_deltasPath))
                    .StartingFrom(_fromDelta)
                    .EndingWith(_toDelta)
                    .GenerateDeploy();
            }

            return 0;
        }
    }
}