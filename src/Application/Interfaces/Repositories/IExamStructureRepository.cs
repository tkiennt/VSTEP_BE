using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IExamStructureRepository
{
    Task<List<ExamStructure>> GetAllAsync();
    Task<ExamStructure?> GetByIdAsync(int id);
    Task<ExamStructure> CreateAsync(ExamStructure entity);
    Task<ExamStructure> UpdateAsync(ExamStructure entity);
    Task DeleteAsync(int id);
}
