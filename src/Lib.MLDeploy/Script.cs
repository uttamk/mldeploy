using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.MLDeploy
{
    internal class Script
    {
        private readonly IScriptRepository _scriptRepository;
        private readonly IDeltaRepository _deltasRepository;
        private long _fromDelta = 0;
        private long _toDelta = Int64.MaxValue;


        internal Script(IScriptRepository scriptRepository, IDeltaRepository deltasRepository)
        {
            _scriptRepository = scriptRepository;
            _deltasRepository = deltasRepository;
        }

        internal Script StartingFrom(string fromDelta)
        {
            Int64.TryParse(fromDelta, out _fromDelta);
           return this;
        }

        internal Script EndingWith(string toDelta)
        {
            Int64.TryParse(toDelta, out _toDelta);

            if (_toDelta == 0)
                _toDelta = Int64.MaxValue;
            return this;
        }

        internal void GenerateDeploy()
        {
            var allDeltas = _deltasRepository.GetAllDeltas();

            var applicableDeltas = allDeltas.OrderBy(d => d.Number)
                .Where(d => d.Number >= _fromDelta)
                .Where(d => d.Number <= _toDelta)
                .ToList();
            if(applicableDeltas.Any())
            {
               Print("[mldeploy] Generating deploy script for deltas ", applicableDeltas);
                _scriptRepository.GenerateDeployScriptFor(applicableDeltas);
            }
            else
            {
                Console.WriteLine("[mldeploy] No deltas applicable to generate script");
            }
        }

        private void Print(string message, IEnumerable<Delta> allDeltas)
        {
            Console.Write(message);

            Console.Write(allDeltas.Count() != 0 ? string.Join(", ", allDeltas.Select(d => d.Number)) : "none");

            Console.WriteLine();
        }

        internal void GenerateRollback()
        {
            var allDeltas = _deltasRepository.GetAllDeltas();

            var applicableDeltas = allDeltas.Where(d => d.Number >= _fromDelta)
                .Where(d => d.Number <= _toDelta)
                .OrderByDescending(d => d.Number)
                .ToList();
            if (applicableDeltas.Any())
            {
                Print("[mldeploy] Generating rollback script for deltas ", applicableDeltas);
                _scriptRepository.GenerateRollBackScriptFor(applicableDeltas);
            }
            else
            {
                Console.WriteLine("[mldeploy] No deltas applicable to generate script");
            }
        }
    }
}