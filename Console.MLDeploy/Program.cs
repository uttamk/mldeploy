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
                    System.Console.WriteLine("USAGE : console.mldeploy.exe xcc://username:password@localhost:9001 C:\\Deltas");
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