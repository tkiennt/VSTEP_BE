using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PartConfiguration : IEntityTypeConfiguration<Part>
    {
        public void Configure(EntityTypeBuilder<Part> builder)
        {
            builder.ToTable("parts");
            
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("part_id");

            builder.Property(e => e.ExamStructureId)
                   .HasColumnName("exam_structure_id")
                   .IsRequired();

            builder.Property(e => e.PartTypeId)
                   .HasColumnName("part_type_id")
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasColumnName("description")
                   .HasMaxLength(500);

            builder.Property(e => e.TimeLimit)
                   .HasColumnName("time_limit");

            builder.Property(e => e.MinWords)
                   .HasColumnName("min_words");

            builder.Property(e => e.MaxWords)
                   .HasColumnName("max_words");

            // Relationships
            builder.HasOne(p => p.ExamStructure)
                   .WithMany(es => es.Parts)
                   .HasForeignKey(p => p.ExamStructureId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.PartType)
                   .WithMany(pt => pt.Parts)
                   .HasForeignKey(p => p.PartTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Topics)
                   .WithOne(t => t.Part)
                   .HasForeignKey(t => t.PartId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.UserSubmissions)
                   .WithOne(us => us.Part)
                   .HasForeignKey(us => us.PartId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => new { e.ExamStructureId, e.PartTypeId })
                   .HasDatabaseName("IX_parts_exam_structure_part_type");
        }
    }
}
