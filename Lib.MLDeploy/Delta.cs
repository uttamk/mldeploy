namespace Lib.MLDeploy
{
    internal class Delta
    {
        internal long Number { get; private set; }
        internal string Path { get; private set; }

        internal Delta(long number, string path)
        {
            Number = number;
            Path = path;
        }
    }
}