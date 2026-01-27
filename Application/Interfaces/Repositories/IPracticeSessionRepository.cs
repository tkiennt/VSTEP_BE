using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPracticeSessionRepository
{
    Task<List<PracticeSession>> GetByUserIdAsync(int userId);
    Task<PracticeSession?> GetByIdAsync(int id);
    Task<PracticeSession> CreateAsync(PracticeSession entity);
}
