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
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("WarehouseName");

            builder.Property(w => w.Location)
                .IsRequired()
                .HasMaxLength(300)
                .HasColumnName("WarehouseLocation");

            builder.Property(w => w.Capacity)
                .HasColumnType("decimal(18,2)")
                .IsRequired()
                .HasColumnName("CapacityInSquareMeters");

            builder.Property(w => w.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(w => w.UpdatedAt)
                .IsRequired(false);

            builder.HasIndex(w => w.Name)
                .IsUnique()
                .HasDatabaseName("IX_Warehouses_Name");

            // Query Filter for soft delete
            builder.HasQueryFilter(w => !w.IsDeleted);
        }
    }
}
