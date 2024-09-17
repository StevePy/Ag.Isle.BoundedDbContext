using NUnit.Framework;

namespace Ag.Isle.EF.Core.UnitTests
{
    [TestFixture]
    public class OptionFinder_WhenAppendingOption
    {
        [TestCase("This is a test", "RECOMPILE", "This is a test OPTION (RECOMPILE)")]
        public void ThenOptionIsAddedWhenNotPresent(string statement, string optionValue, string expected)
        {
            var result = OptionFinder.AppendOption(statement, optionValue);
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("This is a test OPTION (RECOMPILE)", "USE HINT('Hola')", "This is a test OPTION (RECOMPILE, USE HINT('Hola'))")]
        [TestCase("This is a test OPTION(RECOMPILE)", "USE HINT('Hola')", "This is a test OPTION (RECOMPILE, USE HINT('Hola'))")]
        [TestCase("This is a test option (RECOMPILE)", "USE HINT('Hola')", "This is a test OPTION (RECOMPILE, USE HINT('Hola'))")]
        public void ThenOptionIsAppendedWhenPresent(string statement, string optionValue, string expected)
        {
            var result = OptionFinder.AppendOption(statement, optionValue);
            Assert.That(result, Is.EqualTo(expected));
        }


    }
}
