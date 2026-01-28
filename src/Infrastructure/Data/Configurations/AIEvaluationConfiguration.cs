using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class AIEvaluationConfiguration : IEntityTypeConfiguration<AIEvaluation>
    {
        public void Configure(EntityTypeBuilder<AIEvaluation> builder)
        {
            builder.ToTable("ai_evaluations");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("evaluation_id");
            builder.Property(e => e.SubmissionId).HasColumnName("submission_id").IsRequired();
            builder.Property(e => e.TotalScore).HasColumnName("total_score");
            builder.Property(e => e.EstimatedLevelId).HasColumnName("estimated_level_id");
            builder.Property(e => e.OverallFeedback).HasColumnName("overall_feedback");
            builder.Property(e => e.Strengths).HasColumnName("strengths");
            builder.Property(e => e.Weaknesses).HasColumnName("weaknesses");
            builder.Property(e => e.Suggestions).HasColumnName("suggestions");
            builder.Property(e => e.EvaluatedAt).HasColumnName("evaluated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(e => e.Submission)
                   .WithOne(s => s.AIEvaluation)
                   .HasForeignKey<AIEvaluation>(e => e.SubmissionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.EstimatedLevel)
                   .WithMany(l => l.AIEvaluations)
                   .HasForeignKey(e => e.EstimatedLevelId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
