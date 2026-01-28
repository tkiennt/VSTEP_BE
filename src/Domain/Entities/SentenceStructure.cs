namespace Domain.Entities;

public class SentenceStructure
{
    public int Id { get; set; }
    public int VocabSetId { get; set; }
    public string Pattern { get; set; } = string.Empty;
    public string? UsageNote { get; set; }
    public string? Example { get; set; }

    // Navigation properties
    public virtual VocabularySet? VocabularySet { get; set; }
}
