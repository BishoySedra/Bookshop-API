using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DataAccess
{
    public class mainContext : DbContext
    {
        public mainContext(DbContextOptions<mainContext> options) : base(options)
        {
        }

        // Here I define the DbSets for my entities
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Here I can configure the model using Fluent API if needed from another assembly called Models.Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Category).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
