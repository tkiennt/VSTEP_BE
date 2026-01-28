namespace Application.DTOs.Learning;

public class VocabularySetResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<VocabularyItemResponse> VocabularyItems { get; set; } = new();
    public List<SentenceStructureResponse> SentenceStructures { get; set; } = new();
}
