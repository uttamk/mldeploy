using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.MLDeploy
{
    internal class Deployer
    {
        private readonly IDeltaRepository _deltaRepository;

        public Deployer(string connectionString, string deltasDir) : this(new DeltaRepository(connectionString, deltasDir))
        {
        }

        internal Deployer(IDeltaRepository deltaRepository)
        {
            _deltaRepository = deltaRepository;
        }

        public void Deploy()
        {
            var allDeltas = _deltaRepository.GetAllDeltas();
            Print("[mldeploy] Deltas found: ", allDeltas);

            var latestDeltaInDatabase = _deltaRepository.GetLatestDeltaInDatabase();

            var applicableDeltas = allDeltas.Where(delta=>delta.Number > latestDeltaInDatabase.Number)
                                            .OrderBy(delta=>delta.Number);


            Print("[mldeploy] Deltas applicable: ", applicableDeltas);

            foreach (var delta in applicableDeltas)
            {
                _deltaRepository.ApplyDelta(delta);
                _deltaRepository.UpdateLatestDeltaAs(delta);
            }
        }

        private void Print(string message, IEnumerable<Delta> allDeltas)
        {
            Console.Write(message);

            Console.Write(allDeltas.Count() != 0 ? string.Join(", ", allDeltas.Select(d => d.Number)) : "none");

            Console.WriteLine();
        }
    }
}