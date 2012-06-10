using System;
using Lib.MLDeploy;
using NUnit.Framework;

namespace UnitTests.MLDeploy
{
    [TestFixture]
    public class DeltaFileNameTest
    {
        [Test]
        public void Should_Throw_Exception_If_the_delta_number_has_no_leading_numbers()
        {
            Assert.Throws(typeof(ArgumentException),
                          ()=>new DeltaFileName("Test Query 123.xqy"));
        }

        [Test]
        public void Should_Parse_the_delta_Number_from_the_intial_numbers_in_the_file_name()
        {
            var deltaNumber = new DeltaFileName("123 Test Query.xqy").DeltaNumber();
            Assert.AreEqual(123L, deltaNumber);
        }

        [Test]
        public void Should_Parse_the_delta_description_from_the_trailing_description_after_the_number_excluding_the_extension()
        {
            var description = new DeltaFileName("123 Test Query.xqy").Description();
            Assert.AreEqual("Test Query", description);
        } 
        
        [Test]
        public void Should_Parse_the_delta_description_as_an_empty_string_if_file_name_is_just_a_number()
        {
            var description = new DeltaFileName("123.xqy").Description();
            Assert.AreEqual(string.Empty, description);
        }
    }
}