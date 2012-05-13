using System.Collections.Generic;
using System.Linq;
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

            _scriptRepository.AssertWasCalled(dr => dr.GenerateDeployScriptFor(allDeltas));
        }

        [Test]
        public void Should_generate_deploy_script_for_when_the_from_delta_is_specified()
        {
            var allDeltas = new List<Delta>
                                {
                                    new Delta(1, ""),
                                    new Delta(2, "")
                                };
            _deltasRepository.Stub(dr => dr.GetAllDeltas()).Return(allDeltas);

            new Script(_scriptRepository, _deltasRepository).StartingFrom("2")
                .GenerateDeploy();

            var expectedDeltas = new List<Delta>()
                                     {
                                         new Delta(2, "")
                                     };
            _scriptRepository.AssertWasCalled(
                dr => dr.GenerateDeployScriptFor(Arg<List<Delta>>.Matches(d => MatchDeltas(d, expectedDeltas))));
        }

        [Test]
        public void Should_generate_deploy_script_for_when_only_the_to_delta_is_specified()
        {
            var allDeltas = new List<Delta>
                                {
                                    new Delta(1, ""),
                                    new Delta(2, ""),
                                    new Delta(3, "")
                                };
            _deltasRepository.Stub(dr => dr.GetAllDeltas()).Return(allDeltas);

            new Script(_scriptRepository, _deltasRepository).EndingWith("2")
                .GenerateDeploy();

            var expectedDeltas = new List<Delta>
                                     {
                                         new Delta(1, ""),
                                         new Delta(2, "")
                                     };
            _scriptRepository.AssertWasCalled(
                dr => dr.GenerateDeployScriptFor(Arg<List<Delta>>.Matches(d => MatchDeltas(d, expectedDeltas))));
        }

        [Test]
        public void Should_generate_deploy_script_when_both_from_and_to_deltas_are_specified()
        {
            var allDeltas = new List<Delta>
                                {
                                    new Delta(1, ""),
                                    new Delta(2, ""),
                                    new Delta(3, ""),
                                    new Delta(4, "")
                                };
            _deltasRepository.Stub(dr => dr.GetAllDeltas()).Return(allDeltas);

            new Script(_scriptRepository, _deltasRepository).StartingFrom("2")
                                                            .EndingWith("3")
                                                            .GenerateDeploy();

            var expectedDeltas = new List<Delta>
                                     {
                                         new Delta(2, ""),
                                         new Delta(3, "")
                                     };
            _scriptRepository.AssertWasCalled(
                dr => dr.GenerateDeployScriptFor(Arg<List<Delta>>.Matches(d => MatchDeltas(d, expectedDeltas))));
        }

        private bool MatchDeltas(List<Delta> deltas, List<Delta> expectedDeltas)
        {
            return deltas.Count == expectedDeltas.Count
                   && !expectedDeltas.Where((t, i) => t.Number != deltas[i].Number).Any();
        }
    }
}