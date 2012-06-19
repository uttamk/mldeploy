using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lib.MLDeploy
{
    internal class ScriptRepository : IScriptRepository
    {
        private readonly string _outputPath;
        private const string DEPLOY_SCRIPT_NAME = "deployscript.xqy";
        private const string ROLLBACK_SCRIPT_NAME = "rollbackscript.xqy";

        internal ScriptRepository(string outputPath)
        {
            _outputPath = outputPath;
        }


        public void GenerateDeployScriptFor(List<Delta> deltas)
        {
            var scriptContent = new StringBuilder();

            foreach (var delta in deltas)
            {
                Console.WriteLine(string.Format("[mldeploy] Appending delta {0}", delta.Number));

                scriptContent.Append(string.Format("(:delta {0} starts:)", delta.Number));
                scriptContent.AppendLine();
                scriptContent.Append(File.ReadAllText(delta.Path));
                scriptContent.AppendLine();
                scriptContent.Append(string.Format("(:delta {0} ends:)", delta.Number));
                scriptContent.AppendLine();
                scriptContent.AppendLine();
            }

            WriteToFile(scriptContent, DEPLOY_SCRIPT_NAME);
        }

        public void GenerateRollBackScriptFor(List<Delta> deltas)
        {
            var scriptContent = new StringBuilder();

            foreach (var delta in deltas)
            {
                Console.WriteLine(string.Format("[mldeploy] Appending rollback delta {0}", delta.Number));

                scriptContent.Append(string.Format("(:rollback for delta {0} starts:)", delta.Number));
                scriptContent.AppendLine();

                string readAllText = File.ReadAllText(delta.Path);
                scriptContent.Append(ParseRollbackString(readAllText));

                scriptContent.AppendLine();
                scriptContent.Append(string.Format("(:rollback for delta {0} ends:)", delta.Number));
                scriptContent.AppendLine();
                scriptContent.AppendLine();
            }

            WriteToFile(scriptContent, ROLLBACK_SCRIPT_NAME);
        }

        private string ParseRollbackString(string readAllText)
        {
            var separators = new[] { "(:Rollback" };
            string[] rollbackContent = readAllText.Split(separators, StringSplitOptions.None);
            if (rollbackContent.Length < 2)
                return string.Empty;
            //Remove the trailing :)
            return rollbackContent[1].Remove(rollbackContent[1].Length-2);
        }

        private void WriteToFile(StringBuilder scriptContent, string fileName)
        {
            if (!Directory.Exists(_outputPath))
                Directory.CreateDirectory(_outputPath);

            File.WriteAllText(string.Format("{0}\\{1}", _outputPath, fileName),scriptContent.ToString());
        }
    }
}