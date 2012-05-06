using System;

namespace Lib.MLDeploy
{
    internal class Deployer
    {
        private readonly IDeltaRepository _deltaRepository;

        internal Deployer(string connectionString, string deltasDir)
        {
            throw new NotImplementedException();
        }

        internal Deployer(IDeltaRepository deltaRepository)
        {
            _deltaRepository = deltaRepository;
        }

        public void Deploy()
        {
            var allDeltas = _deltaRepository.GetAllDeltas();

            foreach (var delta in allDeltas)
            {
                _deltaRepository.ApplyDelta(delta);
                _deltaRepository.UpdateLatestDeltaAs(delta);
            }
        }
    }
}