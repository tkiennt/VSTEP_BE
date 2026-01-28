using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class VocabularySetConfiguration : IEntityTypeConfiguration<VocabularySet>
    {
        public void Configure(EntityTypeBuilder<VocabularySet> builder)
        {
            builder.ToTable("vocabulary_sets");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("vocab_set_id");
            builder.Property(e => e.TopicId).HasColumnName("topic_id").IsRequired();
            builder.Property(e => e.LevelId).HasColumnName("level_id").IsRequired();
            builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
            builder.Property(e => e.Description).HasColumnName("description");

            builder.HasOne(e => e.Topic)
                   .WithMany(t => t.VocabularySets)
                   .HasForeignKey(e => e.TopicId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Level)
                   .WithMany(l => l.VocabularySets)
                   .HasForeignKey(e => e.LevelId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
