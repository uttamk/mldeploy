using System;
using System.Collections.Generic;
using ManyConsole;

namespace Console.MLDeploy
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            
            try
            {
                var commands = new List<ConsoleCommand>
                                      {
                                          new DeployCommand(),
                                          new ScriptCommand(),
                                          new RunScriptCommand()
                                      };
                ConsoleCommandDispatcher.DispatchCommand(commands, args, System.Console.Out);

            }catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex.StackTrace);
                Environment.Exit(-1);
            }

            Environment.Exit(0);

        }
    }
}