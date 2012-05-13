using System.Collections.Generic;

namespace Lib.MLDeploy
{
    internal interface IScriptRepository
    {
        void GenerateDeployScriptFor(List<Delta> deltas);
    }
}