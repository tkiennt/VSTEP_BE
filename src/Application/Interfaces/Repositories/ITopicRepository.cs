using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ITopicRepository
{
    Task<List<Topic>> GetByPartIdAsync(int partId);
    Task<Topic?> GetByIdAsync(int id);
    Task<Topic> CreateAsync(Topic entity);
    Task<Topic> UpdateAsync(Topic entity);
    Task DeleteAsync(int id);
}
