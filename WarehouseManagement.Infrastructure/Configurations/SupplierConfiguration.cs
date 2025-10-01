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
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("SupplierName");

            builder.Property(s => s.ContactPerson)
                .HasMaxLength(100)
                .HasColumnName("ContactPerson");

            builder.Property(s => s.Email)
                .HasMaxLength(100)
                .HasColumnName("EmailAddress");

            builder.Property(s => s.Phone)
                .HasMaxLength(20)
                .HasColumnName("PhoneNumber");

            builder.Property(s => s.Address)
                .HasMaxLength(500)
                .HasColumnName("SupplierAddress");

            builder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(s => s.UpdatedAt)
                .IsRequired(false);

            builder.HasIndex(s => s.Name)
                .IsUnique()
            .HasDatabaseName("IX_Suppliers_Name");

            builder.HasIndex(s => s.Email)
                .HasDatabaseName("IX_Suppliers_Email");

            // Query Filter for soft delete
            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}
