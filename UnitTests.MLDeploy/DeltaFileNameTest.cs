using System;
using Lib.MLDeploy;
using NUnit.Framework;

namespace UnitTests.MLDeploy
{
    [TestFixture]
    public class DeltaFileNameTest
    {
        [Test]
        public void Should_Parse_the_delta_Number_from_the_intial_numbers_in_the_file_name()
        {
            var deltaNumber = new DeltaRepository.DeltaFileName("123 Test Query.xqy").ParseDeltaNumber();
            Assert.AreEqual(123L, deltaNumber);
        } 
        
        [Test]
        public void Should_Throw_Exception_If_the_delta_number_has_no_leading_numbers()
        {
            Assert.Throws(typeof(ArgumentException),
                          ()=>new DeltaRepository.DeltaFileName("Test Query 123.xqy").ParseDeltaNumber());
        }
    }
}