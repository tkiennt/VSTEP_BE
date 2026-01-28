using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class UserSubmissionConfiguration : IEntityTypeConfiguration<UserSubmission>
    {
        public void Configure(EntityTypeBuilder<UserSubmission> builder)
        {
            builder.ToTable("user_submissions");
            
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("submission_id");

            builder.Property(e => e.SessionId)
                   .HasColumnName("session_id")
                   .IsRequired();

            builder.Property(e => e.TopicId)
                   .HasColumnName("topic_id")
                   .IsRequired();

            builder.Property(e => e.PartId)
                   .HasColumnName("part_id")
                   .IsRequired();

            builder.Property(e => e.Content)
                   .HasColumnName("content")
                   .IsRequired();

            builder.Property(e => e.WordCount)
                   .HasColumnName("word_count");

            builder.Property(e => e.EnableHint)
                   .HasColumnName("enable_hint")
                   .IsRequired();

            builder.Property(e => e.SubmittedAt)
                   .HasColumnName("submitted_at")
                   .IsRequired();

            // Relationships
            builder.HasOne(us => us.Session)
                   .WithMany(ps => ps.UserSubmissions)
                   .HasForeignKey(us => us.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(us => us.Topic)
                   .WithMany(t => t.UserSubmissions)
                   .HasForeignKey(us => us.TopicId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(us => us.Part)
                   .WithMany(p => p.UserSubmissions)
                   .HasForeignKey(us => us.PartId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(e => e.SessionId);
            builder.HasIndex(e => e.TopicId);
            builder.HasIndex(e => e.PartId);
            builder.HasIndex(e => new { e.SessionId, e.TopicId });
        }
    }
}
