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

RecompileInterceptor

A DbCommandInterceptor implementation that looks for a comment tag to add an OPTION (RECOMPILE) to the query. Add the interceptor to your DbContext configuration, then you can use the extension method .WithRecompile() to append it to your query.

DisableRowGoalInterceptor

A DbCommandInterceptor implementation that looks for a comment tag to add the Hint for DISABLE_OPTIMIZER_ROWGOAL. Add the interceptor to your DbContext configuration, then you can use the extension method .WithNoRowGoal() to append it to your query.

OptionFinder

A static helper class for going through an SQL statement to look for an OPTION() section and append/insert options. This enables multiple interceptors to append options to an SQL statement.
