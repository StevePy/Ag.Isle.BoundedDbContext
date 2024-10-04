# Ag.Isle.EF.Core
This is a library of components I use for EF Core solutions including an implementation for a bounded DbContext which
uses attributes for registering entity configurations with one or more DbContext. There is a caching solution for 
lookup entities which handles tracking cache reference hookups automatically plus data refreshes.

BoundedDbContext

An attribute-driven Bounded DbContext implementation for Entity Framework

This implemention works with IEntityConfiguration<TEntity> where you add a [BoundedContextRegistration] Attribute to 
nominate which DbContexts should configure this entity. This can be done by DbContext type or name.

For example if we have an entity for Customer which we want to register for just an AppDbContext:

[BoundedContextRegistration(typeof(AppDbContext))]

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>

For something like a User table we might want a full User entity used for an AuthenticationDbContext with a UserSummary 
for all other bounded DbContexts. The attribute supports "*" for all DbContexts and "!" for exclusions. For example:

[BoundedContextRegistration("*", "!AuthenticationDbContext")]

public class UserSummaryConfiguration : IEntityTypeConfiguration<UserSummary>

[BoundedContextRegistration("AuthenticationDbContext")]

public class UserConfiguration : IEntityTypeConfiguration<User>

Having a separate entity definition helps prevent misuse where the UserSummary does not expose authentication-related 
concerns like roles etc. 

EntityCache

An auto-refreshing caching implementation for lookup-type entities which will work with a current DbContext instance
to check for tracked references, prioritizing those, before cloning and attaching a cached entity if available.

ScopedEntityCache

A version of the EntityCache which integrates with the DbContextScope unit of work pattern to automatically integrate
with any DbContext currently in a DbContextScope for read/refresh, tracking cache checks, and attaching.

Interceptors
These demonstrate SQL generation interceptors that can be used to embed comment tag headers to the SQL that EF generates then intercept the execution to locate the tags and inject desired behaviors such as OPTION() into the resulting queries.

RecompileInterceptor

A DbCommandInterceptor implementation that looks for a comment tag to add an OPTION (RECOMPILE) to the query. Add the interceptor to your DbContext configuration, then you can use the extension method .WithRecompile() to append it to your query.

DisableRowGoalInterceptor

A DbCommandInterceptor implementation that looks for a comment tag to add the Hint for DISABLE_OPTIMIZER_ROWGOAL. Add the interceptor to your DbContext configuration, then you can use the extension method .WithNoRowGoal() to append it to your query.

OptionFinder

A static helper class for going through an SQL statement to look for an OPTION() section and append/insert options. This enables multiple interceptors to append options to an SQL statement.

QueryableExtensions

This is an assortment of extension methods to wrap the conditional application of Linq IQueryable extension methods including Where(), OrderBy(), Include(), and ThenInclude(). For instance rather than encoding conditional logic inside a Linq expression for a Where clause, it is better to move the condition outside, such as "if(condition) query = query.Where({expression})". This ensures that the resulting query only includes conditions that are actually used rather than including all conditions which are more complex as they have the conditional component embedded I.e. "Where(x => x.Name == !string.IsNullOrEmpty(nameFilter) ? nameFilter : x.Name)" which is better written as "if (!string.IsNullOrEmpty(nameFilter)) query = query.Where(x => x.Name == nameFilter);" This only sees the filter added if the condition is met. The only issue with this approach is breaking up the whole query expression. It can be easy to accidentally leave off a "query = " from a conditional filter etc. resulting in the filter not actually getting added to the query expression.  The QueryableExpressions include an extension method WhereIf() which can be used to conditionally apply the expression. Now the Where condition can be inlined while externalizing the condition check. "WhereIf(x => x.Name == nameFilter, !string.IsNullOrEmpty(nameFilter))" The filter expression is only applied if the condition is True. Note that there is a conditional IncludeIf(), ThenIf(), OrderByIf(), and OrderByDescendingIf() available as well. 

Note that there will be some behavioral differences between the conditional and their non-conditional counterparts. For instance OrderByIf() returns IQueryable<T> not IOrderedQueryable<T> since it may, or may not apply the ordering. ThenByIf() will work on IQueryable<T> but checks if it is an IOrderableQueryable<T> before applying a ThenBy(). If executed without a previous OrderBy() it will merely treat the expression as an OrderBy(). So for instance you cannot use OrderByIf().ThenBy(). But a ThenByIf() by itself or after a "false" OrderByIf() will be treated as an OrderBy(). IncludeIf() is a bit different, this will always return an IIncludeableQueryable() if successfully applying the Include() or not. Be cautious with conditional includes, these may have some quirky combinations. It isn't something I generally recommend doing vs. using projection.
