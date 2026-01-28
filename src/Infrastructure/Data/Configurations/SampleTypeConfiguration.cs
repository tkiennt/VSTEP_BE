using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class SampleTypeConfiguration : IEntityTypeConfiguration<SampleType>
    {
        public void Configure(EntityTypeBuilder<SampleType> builder)
        {
            builder.ToTable("sample_types");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("sample_type_id");
            builder.Property(e => e.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
            builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(100);
            builder.HasIndex(e => e.Code).IsUnique();
        }
    }
}
