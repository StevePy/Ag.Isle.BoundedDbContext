using NUnit.Framework;

namespace Ag.Isle.EF.Core
{
    [TestFixture]
    public class OptionFinder_WhenStatementContainsOption
    {
        [TestCase("This is a test OPTION (RECOMPILE)", "RECOMPILE")]
        [TestCase("This is another test option (RECOMPILE)", "RECOMPILE")]
        [TestCase("This is a test OPTION(Recompile)", "Recompile")]
        [TestCase("This is another test option(Recompile)", "Recompile")]
        public void ThenResultContainsOptionContents(string statement, string optionContents)
        {
            var result = OptionFinder.FindOptionsInStatement(statement);
            Assert.That(result.OptionFound, Is.True);
            Assert.That(result.OptionContents, Is.EqualTo(optionContents));
        }

        [TestCase("This is another test OPTION(RECOMPILE, USE HINT('Tada'))", "RECOMPILE, USE HINT('Tada')")]
        public void ThenResultWillContainMultipleOptionContentsIncludingBrackets(string statement, string optionContents)
        {
            var result = OptionFinder.FindOptionsInStatement(statement);
            Assert.That(result.OptionFound, Is.True);
            Assert.That(result.OptionContents, Is.EqualTo(optionContents));
        }
    }
}
