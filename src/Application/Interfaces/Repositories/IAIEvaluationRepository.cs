using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAIEvaluationRepository
{
    Task<AIEvaluation?> GetBySubmissionIdAsync(int submissionId);
    Task<AIEvaluation?> GetByIdAsync(int id);
    Task<AIEvaluation> CreateAsync(AIEvaluation entity);
    Task<AIEvaluation> UpdateAsync(AIEvaluation entity);
}
