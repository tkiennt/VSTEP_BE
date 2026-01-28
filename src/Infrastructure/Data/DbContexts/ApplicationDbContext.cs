using Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.DbContexts;

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

        // Apply entity configurations using Fluent API
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new PasswordResetTokenConfiguration());
        modelBuilder.ApplyConfiguration(new LevelConfiguration());
        modelBuilder.ApplyConfiguration(new PartTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PracticeModeConfiguration());
        modelBuilder.ApplyConfiguration(new ExamStructureConfiguration());
        modelBuilder.ApplyConfiguration(new PartConfiguration());
        modelBuilder.ApplyConfiguration(new TopicConfiguration());
        modelBuilder.ApplyConfiguration(new PracticeSessionConfiguration());
        modelBuilder.ApplyConfiguration(new UserSubmissionConfiguration());
    }
}
