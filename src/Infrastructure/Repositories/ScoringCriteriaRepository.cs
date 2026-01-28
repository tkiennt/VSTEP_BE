using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ScoringCriteriaRepository : IScoringCriteriaRepository
{
    private readonly ApplicationDbContext _context;

    public ScoringCriteriaRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<ScoringCriteria>> GetByPartIdAsync(int partId) =>
        await _context.ScoringCriteria.AsNoTracking().Where(x => x.PartId == partId).ToListAsync();

    public async Task<ScoringCriteria?> GetByIdAsync(int id) => await _context.ScoringCriteria.FindAsync(id);
}
