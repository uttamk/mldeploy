namespace Lib.MLDeploy
{
    internal interface IDeployRepository
    {
        Delta GetLatestDeltaInDatabase();
        void ApplyDelta(Delta delta);
        void UpdateLatestDeltaAs(Delta delta);
    }
}   