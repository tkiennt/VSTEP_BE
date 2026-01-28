namespace Domain.Entities;

public class Topic
{
    public int Id { get; set; }
    public int PartId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public string? Context { get; set; }
    public string? Purpose { get; set; }
    public string? RecipientRole { get; set; }
    public int DifficultyLevelId { get; set; }

    // Navigation properties
    public virtual Part? Part { get; set; }
    public virtual Level? DifficultyLevel { get; set; }
    public virtual ICollection<UserSubmission> UserSubmissions { get; set; } = new List<UserSubmission>();
    public virtual ICollection<VocabularySet> VocabularySets { get; set; } = new List<VocabularySet>();
    public virtual ICollection<SampleText> SampleTexts { get; set; } = new List<SampleText>();
    public virtual ICollection<Hint> Hints { get; set; } = new List<Hint>();
}
