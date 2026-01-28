using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPracticeModeRepository
{
    Task<List<PracticeMode>> GetAllAsync();
    Task<PracticeMode?> GetByIdAsync(int id);
}
