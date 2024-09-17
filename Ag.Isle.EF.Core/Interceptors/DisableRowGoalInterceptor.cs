using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Ag.Isle.EF.Core.Interceptors
{
    /// <summary>
    /// Interceptor for checking for a marker to disable the 
    /// row goal in the query inspection. This can result in poor index
    /// selection resulting in slower queries. 
    /// </summary>
    /// <remarks>
    /// When inspecting a query for slow execution you can check 
    /// the index usage and if you are getting a strange index being
    /// picked, try the Disable_Optimizer_RowGoal to see if performance
    /// improves. If so, use the WithNoRowGoal() extension to mark an
    /// EF query with this option.
    /// </remarks>
    public class DisableRowGoalInterceptor : DbCommandInterceptor
    {
        public static string TagWithMarker = "{{NO-ROW-GOAL}}";
        private const string OptionValue = "USE HINT('DISABLE_OPTIMIZER_ROWGOAL')";

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
