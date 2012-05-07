using System;
using System.Collections.Generic;
using System.IO;
using Lib.MLDeploy;
using Marklogic.Xcc;
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
            File.WriteAllText(string.Format("{0}\\1.xqy",path), "blah");
            File.WriteAllText(string.Format("{0}\\2.xqy", path), "blah");

            List<Delta> allDeltas = new DeltaRepository(path, null).GetAllDeltas();

            Assert.AreEqual(2, allDeltas.Count);
            Assert.IsNotNull(allDeltas.Find(d=>d.Number ==1L && d.Path == "..\\Deltas\\1.xqy"));

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
            DeleteInDatabase("shouldapplydelta.xml", connectionString);
            Directory.Delete(path, true);
            
        }

        [Test]
        public void Should_update_latest_delta()
        {
            const string connectionString = "xcc://admin:password@localhost:9001";

            new DeltaRepository(null, connectionString).UpdateLatestDeltaAs(new Delta(2L, "\\.Deltas\\1.xqy"));

            AssertThatTheLatestDeltaIs(2L, connectionString);


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

        private void DeleteInDatabase(string fileName, string uri)
        {
            ContentSource contentSource = ContentSourceFactory.NewContentSource(new Uri(uri));


            using (var session = contentSource.NewSession())
            {
                Request request = session.NewAdhocQuery(string.Format(@"xquery version ""1.0-ml"";
                                                         xdmp:document-delete(""{0}"")", fileName));
                session.SubmitRequest(request).AsString();

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
