using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class LanguageCheckConfiguration : IEntityTypeConfiguration<LanguageCheck>
    {
        public void Configure(EntityTypeBuilder<LanguageCheck> builder)
        {
            builder.ToTable("language_checks");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("check_id");
            builder.Property(e => e.SubmissionId).HasColumnName("submission_id").IsRequired();
            builder.Property(e => e.SpellingErrors).HasColumnName("spelling_errors").HasDefaultValue(0);
            builder.Property(e => e.GrammarErrors).HasColumnName("grammar_errors").HasDefaultValue(0);
            builder.Property(e => e.SyntaxErrors).HasColumnName("syntax_errors").HasDefaultValue(0);
            builder.Property(e => e.Feedback).HasColumnName("feedback");
            builder.Property(e => e.CheckedAt).HasColumnName("checked_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(e => e.Submission)
                   .WithOne(s => s.LanguageCheck)
                   .HasForeignKey<LanguageCheck>(e => e.SubmissionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
