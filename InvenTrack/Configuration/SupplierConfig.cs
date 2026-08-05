using InvenTrack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenTrack.Configuration
{
    public class SupplierConfig : IEntityTypeConfiguration<Supplier>
    {

        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("suppliers");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(s => s.Email)
                   .HasMaxLength(150);

            builder.Property(s => s.Phone)
                   .HasMaxLength(20);

            builder.Property(s => s.Address)
                   .HasMaxLength(250);

            builder.Property(s => s.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(s => s.UpdatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
