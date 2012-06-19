namespace Lib.MLDeploy
{
    internal class Delta
    {
        private readonly string _description;
        internal string Description
        {
            get { return _description ?? string.Empty; }
        }

        internal long Number { get; private set; }
        internal string Path { get; private set; }


        internal Delta(long number, string path, string description)
        {
            Number = number;
            Path = path;
            _description = description;
        }
    }
}