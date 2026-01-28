using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class LevelConfiguration : IEntityTypeConfiguration<Level>
    {
        public void Configure(EntityTypeBuilder<Level> builder)
        {
            builder.ToTable("levels");
            
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("level_id");

            builder.Property(e => e.LevelCode)
                   .HasColumnName("level_code")
                   .HasMaxLength(5)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasColumnName("description")
                   .HasMaxLength(100);

            // Relationships
            builder.HasMany(l => l.Users)
                   .WithOne(u => u.TargetLevel)
                   .HasForeignKey(u => u.TargetLevelId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(l => l.Topics)
                   .WithOne(t => t.DifficultyLevel)
                   .HasForeignKey(t => t.DifficultyLevelId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.LevelCode).IsUnique();
        }
    }
}
