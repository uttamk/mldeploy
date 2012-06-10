using System.Collections.Generic;
using System.IO;
using Lib.MLDeploy;
using NUnit.Framework;

namespace IntegrationTests.MLDeploy
{
    [TestFixture]
    public class DeltaRepositoryTest
    {
        const string Path = "..\\Deltas";


        [SetUp]
        public void Setup()
        {
            DeleteTestDirectory(Path);
            
        }

        [Test]
        public void Should_get_all_detas_from_file_system_when_file_naming_format_is_Number_xqy()
        {

            Directory.CreateDirectory(Path);
            File.WriteAllText(string.Format("{0}\\1.xqy", Path), "blah");
            File.WriteAllText(string.Format("{0}\\2.xqy", Path), "blah");

            List<Delta> allDeltas = new DeltaRepository(Path).GetAllDeltas();

            Assert.AreEqual(2, allDeltas.Count);
            Assert.IsNotNull(allDeltas.Find(d => d.Number == 1L && d.Path == "..\\Deltas\\1.xqy"));
            Assert.IsNotNull(allDeltas.Find(d => d.Number == 2L && d.Path == "..\\Deltas\\2.xqy"));


        }

        [Test]
        public void Should_get_all_detas_from_file_system_when_file_naming_format_is_Number_String_xqy()
        {
            Directory.CreateDirectory(Path);
            File.WriteAllText(string.Format("{0}\\1 Inserting blah.xqy", Path), "blah");
            File.WriteAllText(string.Format("{0}\\2 Inserting kaw.xqy", Path), "kaw");

            List<Delta> allDeltas = new DeltaRepository(Path).GetAllDeltas();

            Assert.AreEqual(2, allDeltas.Count);
            Assert.IsNotNull(allDeltas.Find(d => d.Number == 1L && d.Path == "..\\Deltas\\1 Inserting blah.xqy"));
            Assert.IsNotNull(allDeltas.Find(d => d.Number == 2L && d.Path == "..\\Deltas\\2 Inserting kaw.xqy"));
        }

        [TearDown]
        public void TearDown()
        {
            DeleteTestDirectory(Path);
            
        }

        private void DeleteTestDirectory(string path)
        {
            try
            {
                Directory.Delete(path, true);
                       
            }catch(DirectoryNotFoundException)
            {
                //Swallow exception, the delete just guards against inconsistent test data
            }
        }
    }
}