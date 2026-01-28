using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IScoringCriteriaRepository
{
    Task<List<ScoringCriteria>> GetByPartIdAsync(int partId);
    Task<ScoringCriteria?> GetByIdAsync(int id);
}
