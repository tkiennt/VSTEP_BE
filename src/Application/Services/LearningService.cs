using Application.DTOs.Learning;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Application.Services;

public class LearningService : ILearningService
{
    private readonly IVocabularySetRepository _vocabSetRepository;
    private readonly ISampleTextRepository _sampleTextRepository;

    public LearningService(
        IVocabularySetRepository vocabSetRepository,
        ISampleTextRepository sampleTextRepository)
    {
        _vocabSetRepository = vocabSetRepository;
        _sampleTextRepository = sampleTextRepository;
    }

    public async Task<TopicLearningResourcesResponse> GetTopicResourcesAsync(int topicId, int levelId)
    {
        var vocabSets = await _vocabSetRepository.GetByTopicAndLevelAsync(topicId, levelId);
        var sampleTexts = await _sampleTextRepository.GetByTopicAndLevelAsync(topicId, levelId);

        return new TopicLearningResourcesResponse
        {
            TopicId = topicId,
            VocabularySets = vocabSets.Select(s => new VocabularySetResponse
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                VocabularyItems = s.VocabularyItems.Select(v => new VocabularyItemResponse
                {
                    Id = v.Id,
                    Word = v.Word,
                    Meaning = v.Meaning,
                    ExampleSentence = v.ExampleSentence,
                    PartOfSpeech = v.PartOfSpeech
                }).ToList(),
                SentenceStructures = s.SentenceStructures.Select(st => new SentenceStructureResponse
                {
                    Id = st.Id,
                    Pattern = st.Pattern,
                    UsageNote = st.UsageNote,
                    Example = st.Example
                }).ToList()
            }).ToList(),
            SampleTexts = sampleTexts.Select(t => new SampleTextResponse
            {
                Id = t.Id,
                Title = t.Title,
                Content = t.Content,
                Author = t.Author,
                SampleTypeCode = t.SampleType?.Code ?? string.Empty
            }).ToList()
        };
    }
}
