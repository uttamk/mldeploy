using System.Collections.Generic;

namespace Lib.MLDeploy
{
    internal interface IDeltaRepository
    {
        List<Delta> GetAllDeltas();
    }
}