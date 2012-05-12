namespace Lib.MLDeploy
{
    internal interface IamADeltaScript
    {
        IamADeltaScript From(string deltasPath);
        IamADeltaScript To(string outputpath);
        IamADeltaScript StartingFrom(string fromDelta);
        IamADeltaScript EndingWith(string toDelta);
        void Generate();
    }
}