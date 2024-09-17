namespace Ag.Isle.EF.Core
{
    /// <summary>
    /// Utility class for searching a generated SQL statement from EF for
    /// any existing OPTIONS statement to parse and append/create
    /// for a given option.
    /// </summary>
    internal static class OptionFinder
    {
        private const string OptionString = "OPTION ({data})";
        private const string OptionPlaceholder = "{data}";

        public static OptionFindResult FindOptionsInStatement(string statement)
        {
            const string OptionPrefix1 = "OPTION (";
            const string OptionPrefix2 = "OPTION(";
            int prefixLength;
            if (string.IsNullOrEmpty(statement)) return OptionFindResult.Failure();

            var optionStartIndex = statement.IndexOf(OptionPrefix1, 0, StringComparison.InvariantCultureIgnoreCase);
            if (optionStartIndex == -1)
            {
                optionStartIndex = statement.IndexOf(OptionPrefix2, 0, StringComparison.InvariantCultureIgnoreCase);
                if (optionStartIndex == -1)
                    return OptionFindResult.Failure();
                else
                    prefixLength = OptionPrefix2.Length;
            }
            else
                prefixLength = OptionPrefix1.Length;

            var optionContentStartIndex = optionStartIndex + prefixLength;
            var optionEndIndex = findClosingBracket(statement, optionContentStartIndex);
            if (optionEndIndex <= optionStartIndex)
                throw new InvalidOperationException("Statement contains an invalid OPTION() tag");

            string optionString = statement.Substring(optionStartIndex, optionEndIndex - optionStartIndex + 1); // capture trailing ")"
            string optionContentString = statement.Substring(optionContentStartIndex, optionEndIndex - optionContentStartIndex);

            return OptionFindResult.Success(optionString, optionContentString);
        }

        public static string AppendOption(string statement, string optionValue)
        {
            if (string.IsNullOrEmpty(statement))
                return statement;

            ArgumentException.ThrowIfNullOrEmpty(optionValue, nameof(optionValue));

            var result = FindOptionsInStatement(statement);
            if (!result.OptionFound)
                return statement += " " + OptionString.Replace(OptionPlaceholder, optionValue);
            else
            {
                string newOptions = OptionString.Replace(OptionPlaceholder, result.OptionContents + ", " + optionValue);
                return statement.Replace(result.Options!, newOptions);
            }
        }

        private static int findClosingBracket(string statement, int startIndex)
        {
            const char OpenBracket = '(';
            const char ClosedBracket = ')';

            int bracketCount = 1;
            var statementArray = statement.ToCharArray();

            for (int count = startIndex; count < statementArray.Length; count++)
            {
                if (statementArray[count] == OpenBracket)
                    bracketCount++;
                else if (statementArray[count] == ClosedBracket)
                    bracketCount--;

                if (bracketCount == 0)
                    return count;
            }
            return -1;
        }
    }

    internal class OptionFindResult
    {
        public bool OptionFound { get; private set; }
        public string? Options { get; private set; }
        public string? OptionContents { get; private set; }

        private OptionFindResult() { }

        public static OptionFindResult Success(string options, string optionContents)
        {
            return new OptionFindResult { OptionFound = true, Options = options, OptionContents = optionContents };
        }

        public static OptionFindResult Failure()
        {
            return new OptionFindResult { OptionFound = false };
        }
    }
}
