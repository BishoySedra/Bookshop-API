using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;

namespace DataAccess.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Table and Schema
            builder.ToTable("Categories", schema: "MasterSchema");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Property Configurations
            builder.Property(c => c.catName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.catOrder)
                .IsRequired();

            builder.Ignore(c => c.createdDate);

            builder.Property(c => c.markedAsDeleted)
                .HasColumnName("isDeleted");

            // Indexing on catName
            builder.HasIndex(c => c.catName)
                .IsUnique()
                .HasDatabaseName("IX_Categories_catName");
        }
    }
}