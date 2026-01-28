using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class SentenceStructureConfiguration : IEntityTypeConfiguration<SentenceStructure>
    {
        public void Configure(EntityTypeBuilder<SentenceStructure> builder)
        {
            builder.ToTable("sentence_structures");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("structure_id");
            builder.Property(e => e.VocabSetId).HasColumnName("vocab_set_id").IsRequired();
            builder.Property(e => e.Pattern).HasColumnName("pattern").HasMaxLength(255).IsRequired();
            builder.Property(e => e.UsageNote).HasColumnName("usage_note");
            builder.Property(e => e.Example).HasColumnName("example");

            builder.HasOne(e => e.VocabularySet)
                   .WithMany(s => s.SentenceStructures)
                   .HasForeignKey(e => e.VocabSetId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
