using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Ag.Isle.EF.Core
{
    /// <summary>
    /// Bounded Context is a base class for defining single scope bounded contexts easily within a project.
    /// Entity definitions may be shared between contexts by having each entity declare which contexts
    /// it wishes to register with. Entity Framework only allows one entity definition per table so this allows
    /// a context to nominate a suitable entity configuration for each table it needs.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract partial class BoundedDbContext<T> : DbContext where T : BoundedDbContext<T>
    {
        private readonly DbConnection? _connection = null;

        public static string Name
        {
            get { return typeof(T).Name; }
        }

        public BoundedDbContext(DbContextOptions<T> options) : base(options)
        { }

        /// <summary>
        /// Return a set of Assembly references that contain the entities to be registered.
        /// </summary>
        /// <remarks>
        /// This will default to the assembly that the implementing bounded context resides in. Override if you
        /// want to point your contexts at one or more classes that contain your domain definitions.
        /// </remarks>
        protected virtual IEnumerable<Assembly> EntityAssemblies
        {
            get { return [GetType().Assembly]; }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityAssembly in EntityAssemblies)
                modelBuilder.ApplyConfigurationsFromAssembly(entityAssembly, t =>
                {
                    var attributes = t.GetCustomAttributes(typeof(BoundedContextRegistrationAttribute), false)
                        .Cast<BoundedContextRegistrationAttribute>()
                        .ToList();
                    if (!attributes.Any()) return false;

                    return attributes.Any(a => a.IsRegisteredToContext(Name));
                });

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            // Note: Can add auditing hook here.

            return base.SaveChanges();
        }
    }
}
