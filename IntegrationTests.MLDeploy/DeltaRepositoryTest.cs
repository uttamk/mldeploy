using System;
using System.Collections.Generic;
using System.IO;
using Lib.MLDeploy;
using Marklogic.Xcc;
using NUnit.Framework;
using UnitTests.MLDeploy;

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
            File.WriteAllText(string.Format("{0}\\1.xqy",path), "blah");
            File.WriteAllText(string.Format("{0}\\2.xqy", path), "blah");

            List<Delta> allDeltas = new DeltaRepository(path, null).GetAllDeltas();

            Assert.AreEqual(2, allDeltas.Count);
            Assert.IsNotNull(allDeltas.Find(d=>d.Number == 1L && d.Path == "..\\Deltas\\1.xqy"));

            Directory.Delete(path, true);
         
        }


        [Test]
        public void Should_apply_delta()
        {
            const string path = "..\\Deltas";
            Directory.CreateDirectory(path);
            const string connectionString = "xcc://admin:password@localhost:9001";
            const string file = "..\\Deltas\\1.xqy";
            File.WriteAllText(file, @"xdmp:document-insert(""shouldapplydelta.xml"", <shouldapplydelta>a</shouldapplydelta>)");

            new DeltaRepository(null, connectionString).ApplyDelta(new Delta(1L, file));

            AssertThatTheDeltaWasApplied("<shouldapplydelta>a</shouldapplydelta>", connectionString);

            //CleanUp
            DeleteTheLatestDeltaInTheDatabase(connectionString);
            Directory.Delete(path, true);
            
        }

        [Test]
        public void Should_update_latest_delta()
        {
            const string connectionString = "xcc://admin:password@localhost:9001";

            new DeltaRepository(null, connectionString).UpdateLatestDeltaAs(new Delta(2L, "\\.Deltas\\1.xqy"));

            AssertThatTheLatestDeltaIs(2L, connectionString);


        } 
        
        [Test]
        public void Should_get_latest_delta_as_no_delta_when_the_database_is_fresh()
        {
            const string connectionString = "xcc://admin:password@localhost:9001";
            DeleteTheLatestDeltaInTheDatabase(connectionString);

            Delta latestDelta = new DeltaRepository(null, connectionString).GetLatestDeltaInDatabase();

            Assert.IsInstanceOf(typeof(NoDelta), latestDelta);

        } 
        
        
        [Test]
        public void Should_get_latest_delta_when_there_is_an_existing_delta_in_the_database()
        {
            const string connectionString = "xcc://admin:password@localhost:9001";
            SetupLatestDeltaAs(10L, connectionString);
            Delta latestDelta = new DeltaRepository(null, connectionString).GetLatestDeltaInDatabase();
            Assert.That(typeof(Delta) ==  latestDelta.GetType());
            Assert.AreEqual(10L, latestDelta.Number);

            DeleteTheLatestDeltaInTheDatabase(connectionString);

        }

        private void SetupLatestDeltaAs(long deltaNumber, string connectionString)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(connectionString));


            using (var session = contentSource.NewSession())
            {
                string xquery = string.Format(@"xquery version ""1.0-ml"";
                                              xdmp:document-insert(""/mldeploy/latest.xml"", <LatestDelta xmlns:m=""http://mldeploy.org""><m:Number>{0}</m:Number></LatestDelta>
                                                                   ,()
                                                                   );"
                                              , deltaNumber);
                Request request =session.NewAdhocQuery(xquery);
               session.SubmitRequest(request).AsString();

            }
        }

        private void AssertThatTheLatestDeltaIs(long deltaNumber, string connectionString)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(connectionString));


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

        private void DeleteTheLatestDeltaInTheDatabase(string uri)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(uri));


            using (var session = contentSource.NewSession())
            {
                try
                {
                    Request request = session.NewAdhocQuery(@"xquery version ""1.0-ml"";
                                                         xdmp:document-delete(""/mldeploy/latest.xml"")");
                    session.SubmitRequest(request).AsString();
                }
                catch (Exception)
                {
                    
                    //Do Nothing, since ML will throw exception if document doesn't exist
                    //Guarantees the isolation of the integration tests
                }
                

            }
        }

        private void AssertThatTheDeltaWasApplied(string xmlString, string uri)
        {

            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(uri));


            using (var session = contentSource.NewSession())
            {
                Request request = session.NewAdhocQuery(@"xquery version ""1.0-ml"";
                                                          for $doc in //*:shouldapplydelta return $doc");
                var result = session.SubmitRequest(request).AsString();

                Assert.AreEqual(xmlString, result);
            }
            
        }
    }
}
