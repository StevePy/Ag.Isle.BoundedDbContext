using Microsoft.EntityFrameworkCore;

namespace Ag.Isle.EF.Core
{
    /// <summary>
    /// Extension methods for EF IQueryable to register queries for
    /// supported interceptors.
    /// </summary>
    public static class InterceptorIQueryableExtensions
    {
        /// <summary>
        /// Add the marker tag for the recompile interceptor to add 
        /// the OPTION hint to the EF query.
        /// </summary>
        public static IQueryable<T> WithRecompile<T>(this IQueryable<T> query)
        {
            ArgumentNullException.ThrowIfNull(query, nameof(query));
            return query.TagWith(Interceptors.RecompileInterceptor.TagWithMarker);
        }

        /// <summary>
        /// Add the marker tag for the disable row goal interceptor to add 
        /// the OPTION hint to the EF query.
        /// </summary>
        public static IQueryable<T> WithNoRowGoal<T>(this IQueryable<T> query)
        {
            ArgumentNullException.ThrowIfNull(query, nameof(query));
            return query.TagWith(Interceptors.DisableRowGoalInterceptor.TagWithMarker);
        }
    }
}
