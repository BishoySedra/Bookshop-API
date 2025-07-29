using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;

namespace DataAccess.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Table and Schema
            builder.ToTable("Products", schema: "MasterSchema");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Title: Required, MaxLength(50)
            builder.Property(p => p.Title)
                   .IsRequired()
                   .HasMaxLength(50);

            // Description: Optional, MaxLength(250)
            builder.Property(p => p.Description)
                   .HasMaxLength(250);

            // Author: Required, MaxLength(50)
            builder.Property(p => p.Author)
                   .IsRequired()
                   .HasMaxLength(50);

            // Price: Required, ColumnName = BookPrice
            builder.Property(p => p.Price)
                   .IsRequired()
                   .HasColumnName("BookPrice")
                   .HasColumnType("decimal(10,2)"); // making 10 digits total, 2 after decimal point

            // Validation for price range (1-1000) – done in application-level validation, not DB

            // Relationship one-to-many ==> one Category can have many Products
            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade); // Optional: specify delete behavior

        }
    }
}