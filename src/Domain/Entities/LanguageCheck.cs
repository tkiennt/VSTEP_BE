namespace Domain.Entities;

public class LanguageCheck
{
    public int Id { get; set; }
    public int SubmissionId { get; set; }
    public int SpellingErrors { get; set; }
    public int GrammarErrors { get; set; }
    public int SyntaxErrors { get; set; }
    public string? Feedback { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual UserSubmission? Submission { get; set; }
}
