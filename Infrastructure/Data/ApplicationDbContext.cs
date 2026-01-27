using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<Level> Levels => Set<Level>();
    public DbSet<PartType> PartTypes => Set<PartType>();
    public DbSet<PracticeMode> PracticeModes => Set<PracticeMode>();
    public DbSet<ExamStructure> ExamStructures => Set<ExamStructure>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<PracticeSession> PracticeSessions => Set<PracticeSession>();
    public DbSet<UserSubmission> UserSubmissions => Set<UserSubmission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("user_id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(100);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(150);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(500);
            entity.Property(e => e.Role).HasColumnName("role").HasConversion<int>();
            entity.Property(e => e.TargetLevelId).HasColumnName("target_level_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.ToTable("password_reset_tokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("token_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Token).HasColumnName("token").HasMaxLength(500);
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.Used).HasColumnName("used");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<Level>(e =>
        {
            e.ToTable("levels");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("level_id");
            e.Property(x => x.LevelCode).HasColumnName("level_code").HasMaxLength(5);
            e.Property(x => x.Description).HasColumnName("description").HasMaxLength(100);
        });
        modelBuilder.Entity<PartType>(e =>
        {
            e.ToTable("part_types");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("part_type_id");
            e.Property(x => x.Code).HasColumnName("code").HasMaxLength(20);
            e.Property(x => x.Description).HasColumnName("description").HasMaxLength(100);
        });
        modelBuilder.Entity<PracticeMode>(e =>
        {
            e.ToTable("practice_modes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("mode_id");
            e.Property(x => x.Code).HasColumnName("code").HasMaxLength(30);
            e.Property(x => x.Description).HasColumnName("description").HasMaxLength(100);
        });
        modelBuilder.Entity<ExamStructure>(e =>
        {
            e.ToTable("exam_structures");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("exam_structure_id");
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(100);
            e.Property(x => x.TotalParts).HasColumnName("total_parts");
            e.Property(x => x.Description).HasColumnName("description");
        });
        modelBuilder.Entity<Part>(e =>
        {
            e.ToTable("parts");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("part_id");
            e.Property(x => x.ExamStructureId).HasColumnName("exam_structure_id");
            e.Property(x => x.PartTypeId).HasColumnName("part_type_id");
            e.Property(x => x.Description).HasColumnName("description");
            e.Property(x => x.TimeLimit).HasColumnName("time_limit");
            e.Property(x => x.MinWords).HasColumnName("min_words");
            e.Property(x => x.MaxWords).HasColumnName("max_words");
        });
        modelBuilder.Entity<Topic>(e =>
        {
            e.ToTable("topics");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("topic_id");
            e.Property(x => x.PartId).HasColumnName("part_id");
            e.Property(x => x.TopicName).HasColumnName("topic_name").HasMaxLength(255);
            e.Property(x => x.Context).HasColumnName("context");
            e.Property(x => x.Purpose).HasColumnName("purpose");
            e.Property(x => x.RecipientRole).HasColumnName("recipient_role").HasMaxLength(100);
            e.Property(x => x.DifficultyLevelId).HasColumnName("difficulty_level_id");
        });
        modelBuilder.Entity<PracticeSession>(e =>
        {
            e.ToTable("practice_sessions");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("session_id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.ModeId).HasColumnName("mode_id");
            e.Property(x => x.IsRandom).HasColumnName("is_random");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });
        modelBuilder.Entity<UserSubmission>(e =>
        {
            e.ToTable("user_submissions");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("submission_id");
            e.Property(x => x.SessionId).HasColumnName("session_id");
            e.Property(x => x.TopicId).HasColumnName("topic_id");
            e.Property(x => x.PartId).HasColumnName("part_id");
            e.Property(x => x.Content).HasColumnName("content");
            e.Property(x => x.WordCount).HasColumnName("word_count");
            e.Property(x => x.EnableHint).HasColumnName("enable_hint");
            e.Property(x => x.SubmittedAt).HasColumnName("submitted_at");
        });
    }
}
