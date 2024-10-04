using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using System.Linq.Expressions;

namespace Ag.Isle.EF.Core
{
    /// <summary>
    /// Extensions for conditional Where and Includes.
    /// </summary>
    /// <remarks>
    /// Rather than embedding if(){ } wrappers around Where or Include
    /// statements you can use these WhereIf() and IncludeIf()/ThenIncludeIf()
    /// passing the conditional expression which will apply the Where or
    /// Include/ThenInclude if the condition is true.
    /// 
    /// Credit to Fildor (https://stackoverflow.com/users/982149/fildor) for 
    /// mentioning this approach with a code snippet for a conditional
    /// ThenInclude() extension method. This code library I extrapolated
    /// and made variants for Where, OrderBy, Include, and ThenInclude.
    /// </remarks>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Apply a Where clause conditionally.
        /// </summary>
        public static IQueryable<TEntity> WhereIf<TEntity>(
            this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> whereExpression,
            bool condition) where TEntity : class
        {
            if (condition)
                return source.Where(whereExpression);

            return source;
        }

        /// <summary>
        /// Apply an OrderBy clause conditionally.
        /// </summary>
        public static IQueryable<TEntity> OrderByIf<TEntity, TProperty>(
            this IQueryable<TEntity> source,
            Expression<Func<TEntity, TProperty>> orderByExpression,
            bool condition) where TEntity : class
        {
            if (condition)
                return source.OrderBy(orderByExpression);

            return source;
        }

        /// <summary>
        /// Apply an OrderByDescending clause conditionally.
        /// </summary>
        public static IQueryable<TEntity> OrderByDescendingIf<TEntity, TProperty>(
            this IQueryable<TEntity> source,
            Expression<Func<TEntity, TProperty>> orderByExpression,
            bool condition) where TEntity : class
        {
            if (condition)
                return source.OrderByDescending(orderByExpression);

            return source;
        }

        /// <summary>
        /// Apply a ThenBy clause conditionally.
        /// </summary>
        public static IQueryable<TEntity> ThenByIf<TEntity, TProperty>(
            this IOrderedQueryable<TEntity> source,
            Expression<Func<TEntity, TProperty>> orderByExpression,
            bool condition) where TEntity : class
        {
            if (condition)
                return source.ThenBy(orderByExpression);

            return source;
        }

        /// <summary>
        /// Apply a ThenBy clause conditionally.
        /// </summary>
        public static IQueryable<TEntity> ThenByIf<TEntity, TProperty>(
            this IQueryable<TEntity> source,
            Expression<Func<TEntity, TProperty>> orderByExpression,
            bool condition) where TEntity : class
        {
            if (condition)
                if (typeof(IOrderedQueryable<TEntity>).IsAssignableFrom(source.GetType()))
                    return ((IOrderedQueryable<TEntity>)source).ThenBy(orderByExpression);
                else
                    return source.OrderBy(orderByExpression);

            return source;
        }

        /// <summary>
        /// Apply a ThenByDescending clause conditionally.
        /// </summary>
        public static IQueryable<TEntity> ThenByDescendingIf<TEntity, TProperty>(
            this IQueryable<TEntity> source,
            Expression<Func<TEntity, TProperty>> orderByExpression,
            bool condition) where TEntity : class
        {
            if (condition)
                if (typeof(IOrderedQueryable<TEntity>).IsAssignableFrom(source.GetType()))
                    return ((IOrderedQueryable<TEntity>)source).ThenByDescending(orderByExpression);
                else
                    return source.OrderByDescending(orderByExpression);

            return source;
        }

        /// <summary>
        /// Include the specified relationship conditionally.
        /// </summary>
        public static IIncludableQueryable<TEntity, TProperty> IncludeIf<TEntity, TProperty>(
            this IQueryable<TEntity> source,
            Expression<Func<TEntity, TProperty>> navigationPropertyPath,
            bool condition) where TEntity : class
        {
            if (condition)
                return source.Include(navigationPropertyPath);

            return new IncludableQueryable<TEntity, TProperty>(source);
        }

        /// <summary>
        /// ThenInclude the specified relationship conditionally.
        /// </summary>
        public static IIncludableQueryable<TEntity, TProperty> ThenIncludeIf<TEntity, TPreviousProperty, TProperty>(
                this IIncludableQueryable<TEntity, TPreviousProperty> source,
                Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath,
                bool condition) where TEntity : class
        {
            if (condition)
                return source.ThenInclude(navigationPropertyPath);

            return new IncludableQueryable<TEntity, TProperty>(source);
        }

        /// <summary>
        /// Wrapper for IIncludableQueryable used by EF.
        /// </summary>
        /// <remarks>
        /// No idea why this is marked as private sealed with EF
        /// and exposed solely through the Include/ThenInclude
        /// implementations. To keep further extensions like 
        /// IncludeIf happy we need an IIncludeableQueryable 
        /// implementation available.
        /// </remarks>
        private sealed class IncludableQueryable<TEntity, TProperty>(IQueryable<TEntity> queryable)
            : IIncludableQueryable<TEntity, TProperty>, IAsyncEnumerable<TEntity>
        {
            Type IQueryable.ElementType => queryable.ElementType;

            Expression IQueryable.Expression => queryable.Expression;

            IQueryProvider IQueryable.Provider => queryable.Provider;

            IAsyncEnumerator<TEntity> IAsyncEnumerable<TEntity>.GetAsyncEnumerator(CancellationToken cancellationToken)
                => ((IAsyncEnumerable<TEntity>)queryable).GetAsyncEnumerator(cancellationToken);

            IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() => queryable.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => queryable.GetEnumerator();
        }
    }
}