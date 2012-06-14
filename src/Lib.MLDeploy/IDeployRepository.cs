using System.Collections.Generic;

namespace Lib.MLDeploy
{
    internal interface IDeployRepository
    {
        Delta GetLatestDeltaInDatabase();
        void ApplyDelta(Delta delta);
        void UpdateLatestDeltaAs(Delta delta);
        void GenerateDeployScriptFor(List<Delta> deltas);
    }
}   