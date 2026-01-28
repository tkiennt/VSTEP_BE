using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.ToTable("password_reset_tokens");
            
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("token_id");

            builder.Property(e => e.UserId)
                   .HasColumnName("user_id")
                   .IsRequired();

            builder.Property(e => e.Token)
                   .HasColumnName("token")
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(e => e.ExpiresAt)
                   .HasColumnName("expires_at")
                   .IsRequired();

            builder.Property(e => e.Used)
                   .HasColumnName("used")
                   .IsRequired();

            builder.Property(e => e.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            // Relationships
            builder.HasOne(prt => prt.User)
                   .WithMany(u => u.PasswordResetTokens)
                   .HasForeignKey(prt => prt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.Token).IsUnique();
            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.ExpiresAt);
        }
    }
}
