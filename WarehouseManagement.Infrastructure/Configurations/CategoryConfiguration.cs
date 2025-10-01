using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100)
            .HasColumnName("CategoryName");

            builder.Property(c => c.Description)
                .HasMaxLength(500)
                .HasColumnName("CategoryDescription");

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);

            builder.HasIndex(c => c.Name)
                .IsUnique()
                .HasDatabaseName("IX_Categories_Name");

            // Query Filter for soft delete
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
