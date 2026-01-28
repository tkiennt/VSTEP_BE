using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ExamStructureConfiguration : IEntityTypeConfiguration<ExamStructure>
    {
        public void Configure(EntityTypeBuilder<ExamStructure> builder)
        {
            builder.ToTable("exam_structures");
            
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("exam_structure_id");

            builder.Property(e => e.Name)
                   .HasColumnName("name")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(e => e.TotalParts)
                   .HasColumnName("total_parts")
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasColumnName("description")
                   .HasMaxLength(500);

            // Relationships
            builder.HasMany(es => es.Parts)
                   .WithOne(p => p.ExamStructure)
                   .HasForeignKey(p => p.ExamStructureId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.Name).IsUnique();
        }
    }
}
