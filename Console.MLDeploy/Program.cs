using System;
using Lib.MLDeploy;

namespace Console.MLDeploy
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    System.Console.WriteLine("USAGE : xcc://username:password@host:port deltas_path");
                }
                else
                {
                    new Deployer(args[0], args[1]).Deploy();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.StackTrace);
            }
        }
    }
}