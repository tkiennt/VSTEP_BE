using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PracticeModeConfiguration : IEntityTypeConfiguration<PracticeMode>
    {
        public void Configure(EntityTypeBuilder<PracticeMode> builder)
        {
            builder.ToTable("practice_modes");
            
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("mode_id");

            builder.Property(e => e.Code)
                   .HasColumnName("code")
                   .HasMaxLength(30)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasColumnName("description")
                   .HasMaxLength(100);

            // Relationships
            builder.HasMany(pm => pm.PracticeSessions)
                   .WithOne(ps => ps.Mode)
                   .HasForeignKey(ps => ps.ModeId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.Code).IsUnique();
        }
    }
}
