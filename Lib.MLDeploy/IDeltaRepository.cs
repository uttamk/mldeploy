using System.Collections.Generic;

namespace Lib.MLDeploy
{
    internal interface IDeltaRepository
    {
        List<Delta> GetAllDeltas();
        Delta GetLatestDeltaInDatabase();
        void ApplyDelta(Delta delta);
        void UpdateLatestDeltaAs(Delta delta);
    }
}