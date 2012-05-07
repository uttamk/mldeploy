using System.Collections.Generic;
using Lib.MLDeploy;
using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTests.MLDeploy
{
    [TestFixture]
    public class DeployerTest
    {
        private IDeltaRepository _deltaRepository;

        [SetUp]
        public void Setup()
        {
            _deltaRepository = MockRepository.GenerateMock<IDeltaRepository>();
        }

        [Test]
        public void Should_deploy_all_deltas_for_a_fresh_database()
        {
            var delta1 = new Delta(1L, "C:\\Deltas");
            var delta2 = new Delta(2L, "C:\\Deltas");
            _deltaRepository.Stub(dr => dr.GetAllDeltas()).Return(new List<Delta>
                                                                        {
                                                                            delta1,
                                                                            delta2
                                                                        });
            _deltaRepository.Stub(dr => dr.GetLatestDeltaInDatabase()).Return(new NoDelta());


            new Deployer(_deltaRepository).Deploy();


            _deltaRepository.AssertWasCalled(dr=>dr.ApplyDelta(delta1));
            _deltaRepository.AssertWasCalled(dr=>dr.UpdateLatestDeltaAs(delta1));
            _deltaRepository.AssertWasCalled(dr=>dr.ApplyDelta(delta2));
            _deltaRepository.AssertWasCalled(dr=>dr.UpdateLatestDeltaAs(delta2));


        }

        [Test]
        public void Should_deploy_deltas_newer_than_the_latest_delta_already_in_the_database()
        {
            var delta1 = new Delta(1L, "C:\\Deltas\\1.xqy");
            var delta2 = new Delta(2L, "C:\\Deltas\\2.xqy");
            var delta3 = new Delta(3L, "C:\\Deltas\\3.xqy");
            _deltaRepository.Stub(dr => dr.GetAllDeltas()).Return(new List<Delta>
                                                                        {
                                                                            delta1,
                                                                            delta2,
                                                                            delta3
                                                                        });
            _deltaRepository.Stub(dr => dr.GetLatestDeltaInDatabase()).Return(delta1);



            new Deployer(_deltaRepository).Deploy();


            _deltaRepository.AssertWasNotCalled(dr => dr.ApplyDelta(delta1));
            _deltaRepository.AssertWasNotCalled(dr => dr.UpdateLatestDeltaAs(delta1));

            _deltaRepository.AssertWasCalled(dr => dr.ApplyDelta(delta2));
            _deltaRepository.AssertWasCalled(dr => dr.UpdateLatestDeltaAs(delta2));
            _deltaRepository.AssertWasCalled(dr => dr.ApplyDelta(delta3));
            _deltaRepository.AssertWasCalled(dr => dr.UpdateLatestDeltaAs(delta3));


        }
    }
}
