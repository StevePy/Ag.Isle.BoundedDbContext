using Microsoft.EntityFrameworkCore;

namespace Ag.Isle.EF.Core
{
    public class TestDbContext : DbContext
    {
        private const string ConnectionString = @"Data Source=PORTASOK2\AGISLE;Initial Catalog=AgIsle;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";

        public DbSet<Lookup> Lookups { get; protected set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
