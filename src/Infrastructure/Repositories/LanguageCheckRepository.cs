using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LanguageCheckRepository : ILanguageCheckRepository
{
    private readonly ApplicationDbContext _context;

    public LanguageCheckRepository(ApplicationDbContext context) => _context = context;

    public async Task<LanguageCheck?> GetBySubmissionIdAsync(int submissionId) =>
        await _context.LanguageChecks.AsNoTracking().FirstOrDefaultAsync(x => x.SubmissionId == submissionId);

    public async Task<LanguageCheck> CreateAsync(LanguageCheck entity)
    {
        _context.LanguageChecks.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
