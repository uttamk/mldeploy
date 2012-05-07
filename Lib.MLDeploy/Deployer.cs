namespace Lib.MLDeploy
{
    internal class Deployer
    {
        private readonly IDeltaRepository _deltaRepository;

        internal Deployer(string connectionString, string deltasDir)
        {
            _deltaRepository = new DeltaRepository(deltasDir, connectionString);
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