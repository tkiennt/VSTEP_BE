using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class HintConfiguration : IEntityTypeConfiguration<Hint>
    {
        public void Configure(EntityTypeBuilder<Hint> builder)
        {
            builder.ToTable("hints");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("hint_id");
            builder.Property(e => e.TopicId).HasColumnName("topic_id").IsRequired();
            builder.Property(e => e.LevelId).HasColumnName("level_id").IsRequired();
            builder.Property(e => e.HintTypeId).HasColumnName("hint_type_id").IsRequired();
            builder.Property(e => e.Content).HasColumnName("content").IsRequired();
            builder.Property(e => e.DisplayOrder).HasColumnName("display_order");

            builder.HasOne(e => e.Topic)
                   .WithMany(t => t.Hints)
                   .HasForeignKey(e => e.TopicId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Level)
                   .WithMany(l => l.Hints)
                   .HasForeignKey(e => e.LevelId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.HintType)
                   .WithMany(ht => ht.Hints)
                   .HasForeignKey(e => e.HintTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
