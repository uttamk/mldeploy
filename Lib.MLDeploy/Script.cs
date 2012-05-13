using System;

namespace Lib.MLDeploy
{
    internal class Script
    {
        private readonly IScriptRepository _scriptRepository;
        private readonly IDeltaRepository _deltasRepository;


        public Script(IScriptRepository scriptRepository, IDeltaRepository deltasRepository)
        {
            _scriptRepository = scriptRepository;
            _deltasRepository = deltasRepository;
        }

        internal Script StartingFrom(string fromDelta)
        {
            return this;
        }

        internal Script EndingWith(string toDelta)
        {
            return this;
        }

        internal void GenerateDeploy()
        {
            var allDeltas = _deltasRepository.GetAllDeltas();

            _scriptRepository.GenerateDeployScriptFor(allDeltas);
        }
    }
}