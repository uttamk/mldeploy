using System;
using System.Collections.Generic;
using Lib.MLDeploy;
using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTests.MLDeploy
{
    [TestFixture]
    public class ScriptTest
    {
        private IScriptRepository _scriptRepository;
        private IDeltaRepository _deltasRepository;

        [SetUp]
        public void Setup()
        {
            _scriptRepository = MockRepository.GenerateMock<IScriptRepository>();
            _deltasRepository = MockRepository.GenerateMock<IDeltaRepository>();
        }

        [Test]
        public void Should_generate_deploy_script_for_all_deltas_if_from_and_to_are_not_specified()
        {
            var allDeltas = new List<Delta>
                                {
                                    new Delta(1, ""),
                                    new Delta(2, "")
                                };
            _deltasRepository.Stub(dr => dr.GetAllDeltas()).Return(allDeltas);
            new Script(_scriptRepository, _deltasRepository).GenerateDeploy();

            _scriptRepository.AssertWasCalled(dr=>dr.GenerateDeployScriptFor(allDeltas));
        }
    }
}