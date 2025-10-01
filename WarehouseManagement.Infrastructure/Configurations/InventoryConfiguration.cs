using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Infrastructure.Configurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Quantity)
                .IsRequired();

            builder.Property(i => i.ReservedQuantity)
                .IsRequired();

            builder.Property(i => i.CreatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Warehouse)
                .WithMany(w => w.Inventories)
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite unique index
            builder.HasIndex(i => new { i.ProductId, i.WarehouseId })
                .IsUnique();

            builder.HasQueryFilter(i => !i.IsDeleted);
        }
    }
}
