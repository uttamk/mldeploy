using System;

namespace Lib.MLDeploy
{
    internal class RollBackScript : IamADeltaScript
    {
        public IamADeltaScript From(string deltasPath)
        {
            throw new NotImplementedException();
        }

        public IamADeltaScript To(string outputpath)
        {
            throw new NotImplementedException();
        }

        public IamADeltaScript StartingFrom(string fromDelta)
        {
            throw new NotImplementedException();
        }

        public IamADeltaScript EndingWith(string toDelta)
        {
            throw new NotImplementedException();
        }

        public void Generate()
        {
            throw new NotImplementedException();
        }
    }
}