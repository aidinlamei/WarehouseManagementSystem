using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Infrastructure.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(t => t.Quantity)
                .IsRequired();

            builder.Property(t => t.Reference)
                .HasMaxLength(100)
            .HasColumnName("ReferenceNumber");

            builder.Property(t => t.Description)
                .HasMaxLength(500)
            .HasColumnName("TransactionDescription");

            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(t => t.UpdatedAt)
                .IsRequired(false);

            // Relationships
            builder.HasOne(t => t.Product)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(t => t.Warehouse)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Indexes
            builder.HasIndex(t => t.Type)
            .HasDatabaseName("IX_Transactions_Type");

            builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Transactions_CreatedAt");

            builder.HasIndex(t => new { t.ProductId, t.CreatedAt })
                .HasDatabaseName("IX_Transactions_Product_CreatedAt");

            // Query Filter for soft delete
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
