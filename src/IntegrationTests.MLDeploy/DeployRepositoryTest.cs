using System;
using System.IO;
using Lib.MLDeploy;
using Marklogic.Xcc;
using NUnit.Framework;

namespace IntegrationTests.MLDeploy
{
    [TestFixture]
    public class DeployRepositoryTest
    {
        private const string _connectionString = "xcc://admin:password@localhost:9001";

        [SetUp]
        public void SetUp()
        {
            CleanUpTheDatabase();
            DeleteTheLatestDeltaInTheDatabase();
        }
        
        [TearDown]
        public void Teardown()
        {
            CleanUpTheDatabase();
            DeleteTheLatestDeltaInTheDatabase();
        }

        [Test]
        public void Should_apply_delta()
        {
            const string path = "..\\Deltas";
            Directory.CreateDirectory(path);
            const string file = "..\\Deltas\\1.xqy";
            File.WriteAllText(file, @"xdmp:document-insert(""shouldapplydelta.xml"", <shouldapplydelta>a</shouldapplydelta>)");

            new DeployRepository(_connectionString, null).ApplyDelta(new Delta(1L, file, "test delta"));

            AssertThatTheDeltaWasApplied("<shouldapplydelta>a</shouldapplydelta>");

            //CleanUp
            DeleteTheLatestDeltaInTheDatabase();
            CleanUpTheDatabase();
            Directory.Delete(path, true);
            
        }

        [Test]
        public void Should_update_latest_delta()
        {
            const string connectionString = "xcc://admin:password@localhost:9001";

            new DeployRepository(connectionString, null).UpdateLatestDeltaAs(new Delta(2L, "\\.Deltas\\1.xqy", "test delta"));

            AssertThatTheLatestDeltaNumberIs(2L);
            AssertThatTheLatestDeltaDescriptionIs("test delta");


        }

        [Test]
        public void Should_get_latest_delta_as_no_delta_when_the_database_is_fresh()
        {
            const string connectionString = "xcc://admin:password@localhost:9001";
            DeleteTheLatestDeltaInTheDatabase();

            Delta latestDelta = new DeployRepository(connectionString, null).GetLatestDeltaInDatabase();

            Assert.IsInstanceOf(typeof(NoDelta), latestDelta);

        }


        [Test]
        public void Should_get_latest_delta_when_there_is_an_existing_delta_in_the_database()
        {
            const string connectionString = "xcc://admin:password@localhost:9001";
            SetupLatestDeltaAs(10L, "blah");
            Delta latestDelta = new DeployRepository(connectionString, null).GetLatestDeltaInDatabase();
            Assert.That(typeof(Delta) ==  latestDelta.GetType());
            Assert.AreEqual(10L, latestDelta.Number);
            Assert.AreEqual("blah", latestDelta.Description);

            DeleteTheLatestDeltaInTheDatabase();

        }

        [Test]
        public void Should_get_latest_delta_when_there_is_an_existing_delta_in_the_database_without_any_description()
        {
            const string connectionString = "xcc://admin:password@localhost:9001";
            SetupLatestDeltaAs(10L);
            Delta latestDelta = new DeployRepository(connectionString, null).GetLatestDeltaInDatabase();
            Assert.That(typeof(Delta) ==  latestDelta.GetType());
            Assert.AreEqual(10L, latestDelta.Number);
            Assert.AreEqual(string.Empty, latestDelta.Description);

            DeleteTheLatestDeltaInTheDatabase();

        }

        private void SetupLatestDeltaAs(long deltaNumber)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));


            using (var session = contentSource.NewSession())
            {
                string xquery = string.Format(@"xquery version ""1.0-ml"";
                                              xdmp:document-insert(""/mldeploy/latest.xml"", <LatestDelta xmlns:m=""http://mldeploy.org""><m:Number>{0}</m:Number></LatestDelta>
                                                                   ,()
                                                                   );"
                                              , deltaNumber);
                Request request = session.NewAdhocQuery(xquery);
                session.SubmitRequest(request).AsString();

            }
        }

        private void SetupLatestDeltaAs(long deltaNumber, string description)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));


            using (var session = contentSource.NewSession())
            {
                string xquery = string.Format(@"xquery version ""1.0-ml"";
                                              xdmp:document-insert(""/mldeploy/latest.xml"", <LatestDelta xmlns:m=""http://mldeploy.org""><m:Number>{0}</m:Number><m:Description>{1}</m:Description></LatestDelta>
                                                                   ,()
                                                                   );"
                                              , deltaNumber, description);
                Request request =session.NewAdhocQuery(xquery);
               session.SubmitRequest(request).AsString();

            }
        }

        private void AssertThatTheLatestDeltaNumberIs(long deltaNumber)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));


            using (var session = contentSource.NewSession())
            {
                Request request = session.NewAdhocQuery(@"xquery version ""1.0-ml"";
                                                        declare namespace m=""http://mldeploy.org"";
                                                        for $doc in //*:LatestDelta return $doc/m:Number/text()"
                                                       );
                var result = session.SubmitRequest(request).AsString();

                Assert.AreEqual(Int64.Parse(result), deltaNumber);

            }
        }

        private void AssertThatTheLatestDeltaDescriptionIs(string expectedDescription)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));


            using (var session = contentSource.NewSession())
            {
                Request request = session.NewAdhocQuery(@"xquery version ""1.0-ml"";
                                                        declare namespace m=""http://mldeploy.org"";
                                                        for $doc in //*:LatestDelta return $doc/m:Description/text()"
                                                       );
                var result = session.SubmitRequest(request).AsString();

                Assert.AreEqual(expectedDescription, result);

            }
        }

        private void DeleteTheLatestDeltaInTheDatabase()
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));


            using (var session = contentSource.NewSession())
            {
                try
                {
                    Request request = session.NewAdhocQuery(@"xquery version ""1.0-ml"";
                                                         xdmp:document-delete(""/mldeploy/latest.xml"")");
                    session.SubmitRequest(request).AsString();
                }
                catch
                {
                    
                    //Do Nothing, since ML will throw exception if document doesn't exist
                    //Guarantees the isolation of the integration tests
                }
                

            }
        }

        private void AssertThatTheDeltaWasApplied(string xmlString)
        {

            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));


            using (var session = contentSource.NewSession())
            {
                Request request = session.NewAdhocQuery(@"xquery version ""1.0-ml"";
                                                          for $doc in //*:shouldapplydelta return $doc");
                var result = session.SubmitRequest(request).AsString();

                Assert.AreEqual(xmlString, result);
            }
            
        }

        private void CleanUpTheDatabase()
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(_connectionString));


            using (var session = contentSource.NewSession())
            {
                try
                {
                    Request request =
                        session.NewAdhocQuery(
                            @"xquery version ""1.0-ml"";
                                                         xdmp:document-delete(""shouldapplydelta.xml"")");
                    session.SubmitRequest(request).AsString();
                }
                catch
                {

                    //Do Nothing, since ML will throw exception if document doesn't exist
                    //Guarantees the isolation of the integration tests
                }
            }
        }
    }
}
