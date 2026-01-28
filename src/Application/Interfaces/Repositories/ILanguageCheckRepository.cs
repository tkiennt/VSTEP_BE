using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ILanguageCheckRepository
{
    Task<LanguageCheck?> GetBySubmissionIdAsync(int submissionId);
    Task<LanguageCheck> CreateAsync(LanguageCheck entity);
}
