using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ScoringCriteriaConfiguration : IEntityTypeConfiguration<ScoringCriteria>
    {
        public void Configure(EntityTypeBuilder<ScoringCriteria> builder)
        {
            builder.ToTable("scoring_criteria");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("criteria_id");
            builder.Property(e => e.PartId).HasColumnName("part_id").IsRequired();
            builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            builder.Property(e => e.Description).HasColumnName("description");
            builder.Property(e => e.Weight).HasColumnName("weight").IsRequired();
            builder.Property(e => e.MaxScore).HasColumnName("max_score").HasDefaultValue(10);

            builder.HasOne(e => e.Part)
                   .WithMany(p => p.ScoringCriteria)
                   .HasForeignKey(e => e.PartId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
