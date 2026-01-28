namespace Application.DTOs.Learning;

public class VocabularyItemResponse
{
    public int Id { get; set; }
    public string Word { get; set; } = string.Empty;
    public string? Meaning { get; set; }
    public string? ExampleSentence { get; set; }
    public string? PartOfSpeech { get; set; }
}
