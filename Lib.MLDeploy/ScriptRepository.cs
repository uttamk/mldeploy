using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lib.MLDeploy
{
    internal class ScriptRepository : IScriptRepository
    {
        private readonly string _outputPath;

        public ScriptRepository(string outputPath)
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

            WriteToFile(scriptContent);
        }

        public void GenerateRollBackScriptFor(List<Delta> deltas)
        {
            throw new NotImplementedException();
        }

        private void WriteToFile(StringBuilder scriptContent)
        {
            if (!Directory.Exists(_outputPath))
                Directory.CreateDirectory(_outputPath);

            File.WriteAllText(string.Format("{0}\\deployscript.xqy", _outputPath),scriptContent.ToString());
        }
    }
}