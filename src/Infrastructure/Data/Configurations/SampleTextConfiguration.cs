using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class SampleTextConfiguration : IEntityTypeConfiguration<SampleText>
    {
        public void Configure(EntityTypeBuilder<SampleText> builder)
        {
            builder.ToTable("sample_texts");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("sample_id");
            builder.Property(e => e.TopicId).HasColumnName("topic_id").IsRequired();
            builder.Property(e => e.LevelId).HasColumnName("level_id").IsRequired();
            builder.Property(e => e.SampleTypeId).HasColumnName("sample_type_id").IsRequired();
            builder.Property(e => e.Content).HasColumnName("content").IsRequired();
            builder.Property(e => e.Title).HasColumnName("title").HasMaxLength(200);
            builder.Property(e => e.Author).HasColumnName("author").HasMaxLength(100);

            builder.HasOne(e => e.Topic)
                   .WithMany(t => t.SampleTexts)
                   .HasForeignKey(e => e.TopicId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Level)
                   .WithMany(l => l.SampleTexts)
                   .HasForeignKey(e => e.LevelId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.SampleType)
                   .WithMany(st => st.SampleTexts)
                   .HasForeignKey(e => e.SampleTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
