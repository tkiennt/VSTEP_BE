using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CriteriaScoreConfiguration : IEntityTypeConfiguration<CriteriaScore>
    {
        public void Configure(EntityTypeBuilder<CriteriaScore> builder)
        {
            builder.ToTable("criteria_scores");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("criteria_score_id");
            builder.Property(e => e.EvaluationId).HasColumnName("evaluation_id").IsRequired();
            builder.Property(e => e.CriteriaId).HasColumnName("criteria_id").IsRequired();
            builder.Property(e => e.Score).HasColumnName("score");
            builder.Property(e => e.Feedback).HasColumnName("feedback");

            builder.HasOne(e => e.Evaluation)
                   .WithMany(ev => ev.CriteriaScores)
                   .HasForeignKey(e => e.EvaluationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Criteria)
                   .WithMany(c => c.CriteriaScores)
                   .HasForeignKey(e => e.CriteriaId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
