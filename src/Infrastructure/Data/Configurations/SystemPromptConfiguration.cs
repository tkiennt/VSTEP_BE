using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class SystemPromptConfiguration : IEntityTypeConfiguration<SystemPrompt>
    {
        public void Configure(EntityTypeBuilder<SystemPrompt> builder)
        {
            builder.ToTable("system_prompts");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("prompt_id");
            builder.Property(e => e.PartId).HasColumnName("part_id").IsRequired();
            builder.Property(e => e.LevelId).HasColumnName("level_id").IsRequired();
            builder.Property(e => e.PurposeId).HasColumnName("purpose_id").IsRequired();
            builder.Property(e => e.PromptContent).HasColumnName("prompt_content").IsRequired();
            builder.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            builder.HasOne(e => e.Part)
                   .WithMany(p => p.SystemPrompts)
                   .HasForeignKey(e => e.PartId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Level)
                   .WithMany(l => l.SystemPrompts)
                   .HasForeignKey(e => e.LevelId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Purpose)
                   .WithMany(pp => pp.SystemPrompts)
                   .HasForeignKey(e => e.PurposeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
