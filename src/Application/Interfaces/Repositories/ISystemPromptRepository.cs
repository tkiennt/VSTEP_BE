using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ISystemPromptRepository
{
    Task<SystemPrompt?> GetActivePromptAsync(int partId, int levelId, string purposeCode);
    Task<List<SystemPrompt>> GetAllAsync();
    Task<SystemPrompt?> GetByIdAsync(int id);
    Task<SystemPrompt> CreateAsync(SystemPrompt entity);
    Task UpdateAsync(SystemPrompt entity);
}
