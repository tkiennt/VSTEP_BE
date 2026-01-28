using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PracticeSessionConfiguration : IEntityTypeConfiguration<PracticeSession>
    {
        public void Configure(EntityTypeBuilder<PracticeSession> builder)
        {
            builder.ToTable("practice_sessions");
            
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("session_id");

            builder.Property(e => e.UserId)
                   .HasColumnName("user_id")
                   .IsRequired();

            builder.Property(e => e.ModeId)
                   .HasColumnName("mode_id")
                   .IsRequired();

            builder.Property(e => e.IsRandom)
                   .HasColumnName("is_random")
                   .IsRequired();

            builder.Property(e => e.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();

            // Relationships
            builder.HasOne(ps => ps.User)
                   .WithMany(u => u.PracticeSessions)
                   .HasForeignKey(ps => ps.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ps => ps.Mode)
                   .WithMany(pm => pm.PracticeSessions)
                   .HasForeignKey(ps => ps.ModeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ps => ps.UserSubmissions)
                   .WithOne(us => us.Session)
                   .HasForeignKey(us => us.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.ModeId);
            builder.HasIndex(e => new { e.UserId, e.ModeId });
        }
    }
}
