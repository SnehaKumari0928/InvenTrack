using InvenTrack.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvenTrack.Configuration
{
    public class RefreshTokenConfig: IEntityTypeConfiguration<RefreshToken>
    {

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<RefreshToken> builder)
        {

            builder.ToTable("RefreshTokens");
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Token).IsRequired();
            builder.Property(rt => rt.ExpiresAt).IsRequired();
            builder.Property(rt => rt.IsRevoked).IsRequired();

            builder.HasOne(rt => rt.User)
                .WithMany(rt => rt.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasIndex(rt => rt.Token).IsUnique();
        }
        }
}
