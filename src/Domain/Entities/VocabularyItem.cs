namespace Domain.Entities;

public class VocabularyItem
{
    public int Id { get; set; }
    public int VocabSetId { get; set; }
    public string Word { get; set; } = string.Empty;
    public string? Meaning { get; set; }
    public string? ExampleSentence { get; set; }
    public string? PartOfSpeech { get; set; }

    // Navigation properties
    public virtual VocabularySet? VocabularySet { get; set; }
}
