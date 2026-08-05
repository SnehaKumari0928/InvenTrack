using InvenTrack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenTrack.Configuration
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {

        public void Configure(EntityTypeBuilder<Product> builder)
        {

            builder.ToTable("products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(p => p.SKU)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.HasIndex(p => p.SKU)
                   .IsUnique();

            builder.Property(p => p.Price)
                   .HasPrecision(12, 2)
                   .IsRequired();

            builder.Property(p => p.StockQuantity)
                   .IsRequired();

            builder.ToTable(t =>
                t.HasCheckConstraint("CK_Product_Stock", "\"StockQuantity\" >= 0"));

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.UpdatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(p => p.Supplier)
                   .WithMany(s => s.Products)
                   .HasForeignKey(p => p.SupplierId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
