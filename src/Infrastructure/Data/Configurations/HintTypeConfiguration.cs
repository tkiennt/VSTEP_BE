using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class HintTypeConfiguration : IEntityTypeConfiguration<HintType>
    {
        public void Configure(EntityTypeBuilder<HintType> builder)
        {
            builder.ToTable("hint_types");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("hint_type_id");
            builder.Property(e => e.Code).HasColumnName("code").HasMaxLength(30).IsRequired();
            builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(100);
            builder.HasIndex(e => e.Code).IsUnique();
        }
    }
}
