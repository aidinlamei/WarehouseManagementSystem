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
    public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
        {
            builder.ToTable("PurchaseOrderItems");

            builder.HasKey(poi => poi.Id);

            builder.Property(poi => poi.Quantity)
                .IsRequired();

            builder.Property(poi => poi.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(poi => poi.TotalPrice)
                .HasColumnType("decimal(18,2)")
                .ValueGeneratedOnAddOrUpdate()
                .IsRequired();

            builder.Property(poi => poi.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(poi => poi.UpdatedAt)
                .IsRequired(false);

            // Relationships
            builder.HasOne(poi => poi.PurchaseOrder)
                .WithMany(po => po.Items)
                .HasForeignKey(poi => poi.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne(poi => poi.Product)
                .WithMany()
                .HasForeignKey(poi => poi.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Index
            builder.HasIndex(poi => poi.PurchaseOrderId)
                .HasDatabaseName("IX_PurchaseOrderItems_PurchaseOrderId");

            // Query Filter for soft delete
            builder.HasQueryFilter(poi => !poi.IsDeleted);
        }
    }
}
