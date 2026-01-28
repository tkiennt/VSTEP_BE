using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PartTypeConfiguration : IEntityTypeConfiguration<PartType>
    {
        public void Configure(EntityTypeBuilder<PartType> builder)
        {
            builder.ToTable("part_types");
            
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("part_type_id");

            builder.Property(e => e.Code)
                   .HasColumnName("code")
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasColumnName("description")
                   .HasMaxLength(100);

            // Relationships
            builder.HasMany(pt => pt.Parts)
                   .WithOne(p => p.PartType)
                   .HasForeignKey(p => p.PartTypeId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.Code).IsUnique();
        }
    }
}
