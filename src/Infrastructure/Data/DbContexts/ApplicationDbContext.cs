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
    public DbSet<HintType> HintTypes => Set<HintType>();
    public DbSet<PromptPurpose> PromptPurposes => Set<PromptPurpose>();
    public DbSet<SampleType> SampleTypes => Set<SampleType>();
    public DbSet<VocabularySet> VocabularySets => Set<VocabularySet>();
    public DbSet<VocabularyItem> VocabularyItems => Set<VocabularyItem>();
    public DbSet<SentenceStructure> SentenceStructures => Set<SentenceStructure>();
    public DbSet<SampleText> SampleTexts => Set<SampleText>();
    public DbSet<Hint> Hints => Set<Hint>();
    public DbSet<LanguageCheck> LanguageChecks => Set<LanguageCheck>();
    public DbSet<ScoringCriteria> ScoringCriteria => Set<ScoringCriteria>();
    public DbSet<AIEvaluation> AIEvaluations => Set<AIEvaluation>();
    public DbSet<CriteriaScore> CriteriaScores => Set<CriteriaScore>();
    public DbSet<SystemPrompt> SystemPrompts => Set<SystemPrompt>();

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
        modelBuilder.ApplyConfiguration(new HintTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PromptPurposeConfiguration());
        modelBuilder.ApplyConfiguration(new SampleTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VocabularySetConfiguration());
        modelBuilder.ApplyConfiguration(new VocabularyItemConfiguration());
        modelBuilder.ApplyConfiguration(new SentenceStructureConfiguration());
        modelBuilder.ApplyConfiguration(new SampleTextConfiguration());
        modelBuilder.ApplyConfiguration(new HintConfiguration());
        modelBuilder.ApplyConfiguration(new LanguageCheckConfiguration());
        modelBuilder.ApplyConfiguration(new ScoringCriteriaConfiguration());
        modelBuilder.ApplyConfiguration(new AIEvaluationConfiguration());
        modelBuilder.ApplyConfiguration(new CriteriaScoreConfiguration());
        modelBuilder.ApplyConfiguration(new SystemPromptConfiguration());
    }
}
