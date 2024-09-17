using NUnit.Framework;

namespace Ag.Isle.EF.Core
{
    [TestFixture]
    public class OptionFinder_WhenStatementDoesNotContainOption
    {
        [TestCase("This is test")]
        public void ThenResultHasNoOptionFound(string statement)
        {
            var result = OptionFinder.FindOptionsInStatement(statement);
            Assert.That(result.OptionFound, Is.False);
        }

        [TestCase("This is test option(Incomplete")]
        [TestCase("This is test option((Invalid)")]
        public void ThenAnInvalidFoundOptionIsThrown(string statement)
        {
            Assert.Throws<InvalidOperationException>(() => { var result = OptionFinder.FindOptionsInStatement(statement); });
        }
    }
}
