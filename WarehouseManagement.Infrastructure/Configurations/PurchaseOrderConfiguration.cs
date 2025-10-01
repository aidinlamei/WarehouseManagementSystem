using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Infrastructure.Configurations
{
    public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.ToTable("PurchaseOrders");

            builder.HasKey(po => po.Id);

            builder.Property(po => po.OrderNumber)
                .IsRequired()
                .HasMaxLength(50)
            .HasColumnName("OrderNumber");

            builder.Property(po => po.OrderDate)
                .IsRequired()
            .HasColumnType("datetime2");

            builder.Property(po => po.ExpectedDeliveryDate)
                .IsRequired(false)
                .HasColumnType("datetime2");

            builder.Property(po => po.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(po => po.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(po => po.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(po => po.UpdatedAt)
                .IsRequired(false);

            // Relationships
            builder.HasOne(po => po.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Indexes
            builder.HasIndex(po => po.OrderNumber)
                .IsUnique()
            .HasDatabaseName("IX_PurchaseOrders_OrderNumber");

            builder.HasIndex(po => po.Status)
            .HasDatabaseName("IX_PurchaseOrders_Status");

            builder.HasIndex(po => po.OrderDate)
                .HasDatabaseName("IX_PurchaseOrders_OrderDate");

            // Query Filter for soft delete
            builder.HasQueryFilter(po => !po.IsDeleted);
        }
    }
}
