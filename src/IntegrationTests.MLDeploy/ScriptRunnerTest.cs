using System;
using System.IO;
using Lib.MLDeploy;
using Marklogic.Xcc;
using NUnit.Framework;

namespace IntegrationTests.MLDeploy
{
    [TestFixture]
    public class ScriptRunnerTest
    {
        private const string FilePath = "..\\Deltas\\foo.xqy";
        private const string Path = "..\\Deltas";
        private const string ConnectionString = @"xcc://admin:password@localhost:9001";

        [SetUp]
        public void SetUp()
        {
            DeleteTestDirectory();
            
        }

        [TearDown]
        public void Teardown()
        {
            DeleteTestDirectory();
            DeleteTheDocumentInTheDatabase();
        }

        [Test]
        public void Should_Run_the_script_from_the_given_file()
        {
            Directory.CreateDirectory(Path);
            const string file = FilePath;
            File.WriteAllText(file, @"xdmp:document-insert(""shouldrunquery.xml"", <shouldrunquery>a</shouldrunquery>)");

            new ScriptRunner(ConnectionString, FilePath)
                            .Run();

            AssertThatTheDatabaseHasADocumentWithContent("<shouldrunquery>a</shouldrunquery>");


        }

        private void DeleteTheDocumentInTheDatabase()
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(ConnectionString));


            using (var session = contentSource.NewSession())
            {
                try
                {
                    Request request = session.NewAdhocQuery(@"xquery version ""1.0-ml"";
                                                         xdmp:document-delete(""shouldrunquery.xml"")");
                    session.SubmitRequest(request).AsString();
                }
                catch
                {

                    //Do Nothing, since ML will throw exception if document doesn't exist
                    //Guarantees the isolation of the integration tests
                }


            }
        }

        private void DeleteTestDirectory()
        {
            try
            {
                Directory.Delete(Path, true);

            }
            catch (Exception)
            {
                
                //Deliberate 
            }
        }


        private void AssertThatTheDatabaseHasADocumentWithContent(string xmlString)
        {

            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(ConnectionString));


            using (var session = contentSource.NewSession())
            {
                Request request = session.NewAdhocQuery(@"xquery version ""1.0-ml"";
                                                          for $doc in //*:shouldrunquery return $doc");
                var result = session.SubmitRequest(request).AsString();

                Assert.AreEqual(xmlString, result);
            }

        }
    
    }
}