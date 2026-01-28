using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ExamStructureRepository : IExamStructureRepository
{
    private readonly ApplicationDbContext _context;

    public ExamStructureRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<ExamStructure>> GetAllAsync() =>
        await _context.ExamStructures.AsNoTracking().OrderBy(x => x.Name).ToListAsync();

    public async Task<ExamStructure?> GetByIdAsync(int id) =>
        await _context.ExamStructures.FindAsync(id);

    public async Task<ExamStructure> CreateAsync(ExamStructure entity)
    {
        _context.ExamStructures.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<ExamStructure> UpdateAsync(ExamStructure entity)
    {
        _context.ExamStructures.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _context.ExamStructures.FindAsync(id);
        if (e != null) { _context.ExamStructures.Remove(e); await _context.SaveChangesAsync(); }
    }
}
