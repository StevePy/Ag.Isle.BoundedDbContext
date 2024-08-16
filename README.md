# Ag.Isle.BoundedDbContext
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
