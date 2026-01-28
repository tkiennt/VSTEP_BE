using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("user_id");

            builder.Property(e => e.Name)
                   .HasColumnName("name")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(e => e.Username)
                   .HasColumnName("username")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(e => e.Email)
                   .HasColumnName("email")
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(e => e.PhoneNumber)
                   .HasColumnName("phone_number")
                   .HasMaxLength(20);

            builder.Property(e => e.PasswordHash)
                   .HasColumnName("password_hash")
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(e => e.Role)
                   .HasColumnName("role")
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(e => e.TargetLevelId)
                   .HasColumnName("target_level_id");

            builder.Property(e => e.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            builder.Property(e => e.UpdatedAt)
                   .HasColumnName("updated_at");

            builder.Property(e => e.IsActive)
                   .HasColumnName("is_active")
                   .IsRequired();

            // Relationships
            builder.HasOne(u => u.TargetLevel)
                   .WithMany(l => l.Users)
                   .HasForeignKey(u => u.TargetLevelId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(e => e.Username).IsUnique();
            builder.HasIndex(e => e.Email).IsUnique();
            builder.HasIndex(e => e.PhoneNumber).IsUnique().HasFilter("[phone_number] IS NOT NULL");
        }
    }
}
