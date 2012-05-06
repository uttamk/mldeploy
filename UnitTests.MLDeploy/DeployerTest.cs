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
            _deltaRepository.Expect(dr => dr.GetAllDeltas()).Return(new List<Delta>
                                                                        {
                                                                            delta1,
                                                                            delta2
                                                                        });


            new Deployer(_deltaRepository).Deploy();


            _deltaRepository.AssertWasCalled(dr=>dr.ApplyDelta(delta1));
            _deltaRepository.AssertWasCalled(dr=>dr.UpdateLatestDeltaAs(delta1));
            _deltaRepository.AssertWasCalled(dr=>dr.ApplyDelta(delta2));
            _deltaRepository.AssertWasCalled(dr=>dr.UpdateLatestDeltaAs(delta2));


        }
    }
}
