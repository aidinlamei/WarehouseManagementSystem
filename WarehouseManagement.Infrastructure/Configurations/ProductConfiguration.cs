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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("ProductName");

            builder.Property(p => p.Description)
                .HasMaxLength(1000)
                .HasColumnName("ProductDescription");

            builder.Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("StockKeepingUnit");

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.Cost)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.MinimumStockLevel)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(p => p.MaximumStockLevel)
                .HasDefaultValue(1000)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false);

            builder.HasIndex(p => p.SKU)
                .IsUnique()
                .HasDatabaseName("IX_Products_SKU");

            builder.HasIndex(p => p.Name)
                .HasDatabaseName("IX_Products_Name");

            // Relationships
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Query Filter for soft delete
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
