using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IVocabularySetRepository
{
    Task<List<VocabularySet>> GetByTopicAndLevelAsync(int topicId, int levelId);
    Task<VocabularySet?> GetByIdAsync(int id);
    Task<VocabularySet> CreateAsync(VocabularySet entity);
    Task<VocabularySet> UpdateAsync(VocabularySet entity);
    Task DeleteAsync(int id);
}
