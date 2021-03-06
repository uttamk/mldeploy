﻿using System.Collections.Generic;
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
        public void Should_generate_deploy_script_for_all_deltas_in_right_order_if_from_and_to_are_not_specified()
        {
            var allDeltas = new List<Delta>
                                {
                                    new Delta(2, "", "test delta 2"),
                                    new Delta(1, "", "test delta 1")
                                };
            _deltasRepository.Stub(dr => dr.GetAllDeltas()).Return(allDeltas);
            new Script(_scriptRepository, _deltasRepository).GenerateDeploy();

            var expectedDeltas = new List<Delta>
                                {
                                    new Delta(1, "", "test delta 1"),
                                    new Delta(2, "", "test delta 2")
                                };
            _scriptRepository.AssertWasCalled(dr => dr.GenerateDeployScriptFor(Arg<List<Delta>>.Matches(d=>MatchDeltas(d, expectedDeltas))));
        }

        [Test]
        public void Should_generate_rollback_script_for_all_deltas_in_descending_order_if_from_and_to_are_not_specified()
        {
            var allDeltas = new List<Delta>
                                {
                                    new Delta(1, "", "test delta 1"),
                                    new Delta(2, "", "test delta 2")
                                };
            _deltasRepository.Stub(dr => dr.GetAllDeltas()).Return(allDeltas);
            new Script(_scriptRepository, _deltasRepository).GenerateRollback();

            var expectedDeltas = new List<Delta>
                                {
                                    new Delta(2, "", "test delta 2"),
                                    new Delta(1, "", "test delta 1")
                                }; 

            _scriptRepository.AssertWasCalled(dr => dr.GenerateRollBackScriptFor(Arg<List<Delta>>.Matches(d=>MatchDeltas(d, expectedDeltas))));
        }

        [Test]
        public void Should_generate_deploy_script_for_when_the_from_delta_is_specified()
        {
            var allDeltas = new List<Delta>
                                {
                                    new Delta(1, "", "test delta 1"),
                                    new Delta(2, "", "test delta 2")
                                };
            _deltasRepository.Stub(dr => dr.GetAllDeltas()).Return(allDeltas);

            new Script(_scriptRepository, _deltasRepository).StartingFrom("2")
                .GenerateDeploy();

            var expectedDeltas = new List<Delta>
                                     {
                                         new Delta(2, "", "test delta 2")
                                     };
            _scriptRepository.AssertWasCalled(
                dr => dr.GenerateDeployScriptFor(Arg<List<Delta>>.Matches(d => MatchDeltas(d, expectedDeltas))));
        }
        
        [Test]
        public void Should_generate_rollback_script_for_when_the_from_delta_is_specified()
        {
            var allDeltas = new List<Delta>
                                {
                                    new Delta(1, "", "test delta 1"),
                                    new Delta(2, "", "test delta 2"),
                                    new Delta(3, "", "test delta 3")
                                };
            _deltasRepository.Stub(dr => dr.GetAllDeltas()).Return(allDeltas);

            new Script(_scriptRepository, _deltasRepository).StartingFrom("2")
                                                            .GenerateRollback() ;

            var expectedDeltas = new List<Delta>
                                     {
                                         new Delta(3, "", "test delta"),
                                         new Delta(2, "", "test delta")
                                     };
            _scriptRepository.AssertWasCalled(
                dr => dr.GenerateRollBackScriptFor(Arg<List<Delta>>.Matches(d => MatchDeltas(d, expectedDeltas))));
        }

        [Test]
        public void Should_generate_deploy_script_for_when_only_the_to_delta_is_specified()
        {
            var allDeltas = new List<Delta>
                                {
                                    new Delta(1, "", "test delta"),
                                    new Delta(2, "", "test delta"),
                                    new Delta(3, "", "test delta")
                                };
            _deltasRepository.Stub(dr => dr.GetAllDeltas()).Return(allDeltas);

            new Script(_scriptRepository, _deltasRepository).EndingWith("2")
                .GenerateDeploy();

            var expectedDeltas = new List<Delta>
                                     {
                                         new Delta(1, "", "test delta"),
                                         new Delta(2, "", "test delta")
                                     };
            _scriptRepository.AssertWasCalled(
                dr => dr.GenerateDeployScriptFor(Arg<List<Delta>>.Matches(d => MatchDeltas(d, expectedDeltas))));
        }

        [Test]
        public void Should_generate_rollback_script_for_when_only_the_to_delta_is_specified()
        {
            var allDeltas = new List<Delta>
                                {
                                    new Delta(1, "", "test delta"),
                                    new Delta(2, "", "test delta"),
                                    new Delta(3, "","test delta"),
                                    new Delta(4, "", "test delta")
                                };
            _deltasRepository.Stub(dr => dr.GetAllDeltas()).Return(allDeltas);

            new Script(_scriptRepository, _deltasRepository).EndingWith("3")
                                                            .GenerateRollback();

            var expectedDeltas = new List<Delta>
                                     {
                                         new Delta(3, "", "test delta"),
                                         new Delta(2, "","test delta"),
                                         new Delta(1, "", "test delta")
                                     };
            _scriptRepository.AssertWasCalled(
                dr => dr.GenerateRollBackScriptFor(Arg<List<Delta>>.Matches(d => MatchDeltas(d, expectedDeltas))));
        }

        [Test]
        public void Should_generate_deploy_script_when_both_from_and_to_deltas_are_specified()
        {
            var allDeltas = new List<Delta>
                                {
                                    new Delta(1, "", "test delta"),
                                    new Delta(2, "", "test delta"),
                                    new Delta(3, "", "test delta"),
                                    new Delta(4, "", "test delta")
                                };
            _deltasRepository.Stub(dr => dr.GetAllDeltas()).Return(allDeltas);

            new Script(_scriptRepository, _deltasRepository).StartingFrom("2")
                                                            .EndingWith("3")
                                                            .GenerateDeploy();

            var expectedDeltas = new List<Delta>
                                     {
                                         new Delta(2, "", "test delta"),
                                         new Delta(3, "", "test delta")
                                     };
            _scriptRepository.AssertWasCalled(
                dr => dr.GenerateDeployScriptFor(Arg<List<Delta>>.Matches(d => MatchDeltas(d, expectedDeltas))));
        }
        
        public void Should_generate_rollback_script_when_both_from_and_to_deltas_are_specified()
        {
            var allDeltas = new List<Delta>
                                {
                                    new Delta(1, "", "test delta"),
                                    new Delta(2, "", "test delta"),
                                    new Delta(3, "", "test delta"),
                                    new Delta(4, "", "test delta")
                                };
            _deltasRepository.Stub(dr => dr.GetAllDeltas()).Return(allDeltas);

            new Script(_scriptRepository, _deltasRepository).StartingFrom("2")
                                                            .EndingWith("3")
                                                            .GenerateRollback();

            var expectedDeltas = new List<Delta>
                                     {
                                         new Delta(3, "", "test delta"),
                                         new Delta(2, "", "test delta")
                                     };
            _scriptRepository.AssertWasCalled(
                dr => dr.GenerateRollBackScriptFor(Arg<List<Delta>>.Matches(d => MatchDeltas(d, expectedDeltas))));
        }

        private bool MatchDeltas(List<Delta> deltas, List<Delta> expectedDeltas)
        {
            return deltas.Count == expectedDeltas.Count
                   && !expectedDeltas.Where((t, i) => t.Number != deltas[i].Number).Any();
        }
    }
}