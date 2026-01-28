namespace Application.DTOs.Learning;

public class TopicLearningResourcesResponse
{
    public int TopicId { get; set; }
    public List<VocabularySetResponse> VocabularySets { get; set; } = new();
    public List<SampleTextResponse> SampleTexts { get; set; } = new();
}
