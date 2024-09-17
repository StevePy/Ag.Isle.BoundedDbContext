using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Ag.Isle.EF.Core.Interceptors
{
    /// <summary>
    /// Interceptor for checking for a marker to force a query 
    /// execution recompile. 
    /// </summary>
    /// <remarks>
    /// When inspecting a query for slow execution in one environment vs.
    /// another you might opt to force a recompile to avoid parameter 
    /// sniffing issues with commonly run queries where parameter differences
    /// could benefit from different execution plans and a sub-optimal one
    /// is returned.
    /// </remarks>
    public class RecompileInterceptor : DbCommandInterceptor
    {
        public static string TagWithMarker = "{{RECOMPILE}}";
        private const string OptionValue = "RECOMPILE";

        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            ManipulateCommand(command);

            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            ManipulateCommand(command);

            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }

        private static void ManipulateCommand(DbCommand command)
        {
            var lines = command.CommandText.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);
            bool matchFound = false;
            foreach (var line in lines)
            {
                if (!line.StartsWith("--"))
                    break;
                matchFound = line.StartsWith($"-- {TagWithMarker}");
                if (matchFound)
                    break;
            }
            if (matchFound)
                command.CommandText = OptionFinder.AppendOption(command.CommandText, OptionValue);
        }
    }
}
