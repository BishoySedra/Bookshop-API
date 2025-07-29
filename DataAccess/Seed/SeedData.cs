using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DataAccess.Seed
{
    public static class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, catName = "Science", catOrder = 1 },
                new Category { Id = 2, catName = "Technology", catOrder = 2 },
                new Category { Id = 3, catName = "History", catOrder = 3 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Title = "Book 1", Description = "Desc 1", Author = "Author 1", Price = 99.99M, CategoryId = 1 },
                new Product { Id = 2, Title = "Book 2", Description = "Desc 2", Author = "Author 2", Price = 49.50M, CategoryId = 2 }
            );
        }
    }
}
