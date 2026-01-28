using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IUserSubmissionRepository
{
    Task<List<UserSubmission>> GetBySessionIdAsync(int sessionId);
    Task<List<UserSubmission>> GetByUserIdAsync(int userId);
    Task<UserSubmission?> GetByIdAsync(int id);
    Task<UserSubmission> CreateAsync(UserSubmission entity);
}
