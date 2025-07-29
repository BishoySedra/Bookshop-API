using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class mainContext : DbContext
    {
        public mainContext(DbContextOptions<mainContext> options) : base(options)
        {
        }

        // Here I define the DbSets for my entities

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Here I can configure the model using Fluent API if needed

            base.OnModelCreating(modelBuilder);
        }
    }
}
