using System.Collections.Generic;
using System.Linq;

namespace Lib.MLDeploy
{
    internal class Deployer
    {
        private readonly IDeployRepository _deployRepository;
        private readonly IDeltaRepository _deltaRepository;

        internal Deployer(string connectionString, string deltasDir) : this(new DeployRepository(connectionString, deltasDir), new DeltaRepository(deltasDir))
        {
        }

        internal Deployer(IDeployRepository deployRepository, IDeltaRepository deltaRepository)
        {
            _deployRepository = deployRepository;
            _deltaRepository = deltaRepository;
        }

        internal void Deploy()
        {
            var allDeltas = _deltaRepository.GetAllDeltas()
                                            .OrderBy(d=>d.Number);
            Print("[mldeploy] Deltas found: ", allDeltas);

            var latestDeltaInDatabase = _deployRepository.GetLatestDeltaInDatabase();

            var applicableDeltas = allDeltas.Where(delta=>delta.Number > latestDeltaInDatabase.Number)
                                            .OrderBy(delta=>delta.Number);


            Print("[mldeploy] Deltas applicable: ", applicableDeltas);

            foreach (var delta in applicableDeltas)
            {
                _deployRepository.ApplyDelta(delta);
                _deployRepository.UpdateLatestDeltaAs(delta);
            }
        }

        private void Print(string message, IEnumerable<Delta> allDeltas)
        {
            System.Console.Write(message);

            System.Console.Write(allDeltas.Count() != 0 ? string.Join(", ", allDeltas.Select(d => d.Number)) : "none");

            System.Console.WriteLine();
        }
    }
}