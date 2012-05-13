﻿using System.Collections.Generic;
using System.IO;
using Lib.MLDeploy;
using NUnit.Framework;

namespace IntegrationTests.MLDeploy
{
    [TestFixture]
    public class ScriptRepositoryTest
    {
        [Test]
        public void Should_generate_script_file_for_given_deltas()
        {
            const string path = "..\\Deltas";
            const string outputPath = "..\\Deltas\\Script";

            Directory.CreateDirectory(path);
            File.WriteAllText(string.Format("{0}\\1.xqy", path), @"xdmp:document-insert(""blah.xml"", <blah></blah>);");
            File.WriteAllText(string.Format("{0}\\2.xqy", path), @"xdmp:document-insert(""kaw.xml"", <kaw></kaw>);");

            new ScriptRepository(outputPath).GenerateDeployScriptFor(new List<Delta>
                                                                         {
                                                                             new Delta(1L, "..\\Deltas\\1.xqy"),
                                                                             new Delta(2L, "..\\Deltas\\2.xqy")
                                                                         });



            var scriptContent = File.ReadAllText(string.Format("{0}\\deployscript.xqy", outputPath));
            Assert.IsNotNull(scriptContent);
            Assert.That(scriptContent.Contains(@"xdmp:document-insert(""blah.xml"", <blah></blah>);"));
            Assert.That(scriptContent.Contains(@"xdmp:document-insert(""kaw.xml"", <kaw></kaw>);"));

            Directory.Delete(path, true);

        }
    }
}