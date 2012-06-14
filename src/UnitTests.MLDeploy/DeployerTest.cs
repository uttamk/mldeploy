using System.Collections.Generic;
using Lib.MLDeploy;
using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTests.MLDeploy
{
    [TestFixture]
    public class DeployerTest
    {
        private IDeployRepository _deployRepository;
        private IDeltaRepository _deltaRepository;

        [SetUp]
        public void Setup()
        {
            _deployRepository = MockRepository.GenerateMock<IDeployRepository>();
            _deltaRepository = MockRepository.GenerateMock<IDeltaRepository>();
        }

        [Test]
        public void Should_deploy_all_deltas_for_a_fresh_database()
        {
            var delta1 = new Delta(1L, "C:\\Deltas", "test delta");
            var delta2 = new Delta(2L, "C:\\Deltas", "test delta");
            _deltaRepository.Stub(dr => dr.GetAllDeltas()).Return(new List<Delta>
                                                                        {
                                                                            delta1,
                                                                            delta2
                                                                        });
            _deployRepository.Stub(dr => dr.GetLatestDeltaInDatabase()).Return(new NoDelta());


            new Deployer(_deployRepository, _deltaRepository).Deploy();


            _deployRepository.AssertWasCalled(dr=>dr.ApplyDelta(delta1));
            _deployRepository.AssertWasCalled(dr=>dr.UpdateLatestDeltaAs(delta1));
            _deployRepository.AssertWasCalled(dr=>dr.ApplyDelta(delta2));
            _deployRepository.AssertWasCalled(dr=>dr.UpdateLatestDeltaAs(delta2));


        }

        [Test]
        public void Should_deploy_deltas_newer_than_the_latest_delta_already_in_the_database()
        {
            var delta1 = new Delta(1L, "C:\\Deltas\\1.xqy", "test delta 1");
            var delta2 = new Delta(2L, "C:\\Deltas\\2.xqy", "test delta 2");
            var delta3 = new Delta(3L, "C:\\Deltas\\3.xqy", "test delta 3");
            _deltaRepository.Stub(dr => dr.GetAllDeltas()).Return(new List<Delta>
                                                                        {
                                                                            delta1,
                                                                            delta2,
                                                                            delta3
                                                                        });
            _deployRepository.Stub(dr => dr.GetLatestDeltaInDatabase()).Return(delta1);



            new Deployer(_deployRepository, _deltaRepository).Deploy();


            _deployRepository.AssertWasNotCalled(dr => dr.ApplyDelta(delta1));
            _deployRepository.AssertWasNotCalled(dr => dr.UpdateLatestDeltaAs(delta1));

            _deployRepository.AssertWasCalled(dr => dr.ApplyDelta(delta2));
            _deployRepository.AssertWasCalled(dr => dr.UpdateLatestDeltaAs(delta2));
            _deployRepository.AssertWasCalled(dr => dr.ApplyDelta(delta3));
            _deployRepository.AssertWasCalled(dr => dr.UpdateLatestDeltaAs(delta3));


        }
    }
}
