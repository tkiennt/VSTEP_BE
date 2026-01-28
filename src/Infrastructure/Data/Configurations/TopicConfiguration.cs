using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class TopicConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.ToTable("topics");
            
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("topic_id");

            builder.Property(e => e.PartId)
                   .HasColumnName("part_id")
                   .IsRequired();

            builder.Property(e => e.TopicName)
                   .HasColumnName("topic_name")
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(e => e.Context)
                   .HasColumnName("context")
                   .HasMaxLength(1000);

            builder.Property(e => e.Purpose)
                   .HasColumnName("purpose")
                   .HasMaxLength(500);

            builder.Property(e => e.RecipientRole)
                   .HasColumnName("recipient_role")
                   .HasMaxLength(100);

            builder.Property(e => e.DifficultyLevelId)
                   .HasColumnName("difficulty_level_id")
                   .IsRequired();

            // Relationships
            builder.HasOne(t => t.Part)
                   .WithMany(p => p.Topics)
                   .HasForeignKey(t => t.PartId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.DifficultyLevel)
                   .WithMany(l => l.Topics)
                   .HasForeignKey(t => t.DifficultyLevelId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.UserSubmissions)
                   .WithOne(us => us.Topic)
                   .HasForeignKey(us => us.TopicId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.TopicName);
            builder.HasIndex(e => e.PartId);
            builder.HasIndex(e => e.DifficultyLevelId);
        }
    }
}
