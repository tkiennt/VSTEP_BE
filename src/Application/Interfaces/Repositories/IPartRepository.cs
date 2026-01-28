using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPartRepository
{
    Task<List<Part>> GetByExamStructureIdAsync(int examStructureId);
    Task<Part?> GetByIdAsync(int id);
    Task<Part> CreateAsync(Part entity);
    Task<Part> UpdateAsync(Part entity);
    Task DeleteAsync(int id);
}
