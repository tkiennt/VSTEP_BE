using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PromptPurposeConfiguration : IEntityTypeConfiguration<PromptPurpose>
    {
        public void Configure(EntityTypeBuilder<PromptPurpose> builder)
        {
            builder.ToTable("prompt_purposes");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("purpose_id");
            builder.Property(e => e.Code).HasColumnName("code").HasMaxLength(30).IsRequired();
            builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(100);
            builder.HasIndex(e => e.Code).IsUnique();
        }
    }
}
