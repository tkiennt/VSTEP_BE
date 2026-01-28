using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ISampleTextRepository
{
    Task<List<SampleText>> GetByTopicAndLevelAsync(int topicId, int levelId);
    Task<SampleText?> GetByIdAsync(int id);
    Task<SampleText> CreateAsync(SampleText entity);
    Task<SampleText> UpdateAsync(SampleText entity);
    Task DeleteAsync(int id);
}
