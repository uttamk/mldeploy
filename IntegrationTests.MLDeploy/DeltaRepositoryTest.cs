using System.Collections.Generic;
using System.IO;
using Lib.MLDeploy;
using NUnit.Framework;

namespace IntegrationTests.MLDeploy
{
    [TestFixture]
    public class DeltaRepositoryTest
    {

        [Test]
        public void Should_get_all_detas_from_file_system()
        {
            const string path = "..\\Deltas";

            Directory.CreateDirectory(path);
            File.WriteAllText(string.Format("{0}\\1.xqy", path), "blah");
            File.WriteAllText(string.Format("{0}\\2.xqy", path), "blah");

            List<Delta> allDeltas = new DeltaRepository(path).GetAllDeltas();

            Assert.AreEqual(2, allDeltas.Count);
            Assert.IsNotNull(allDeltas.Find(d => d.Number == 1L && d.Path == "..\\Deltas\\1.xqy"));

            Directory.Delete(path, true);

        }

    }
}