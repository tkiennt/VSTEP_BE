using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SystemPromptRepository : ISystemPromptRepository
{
    private readonly ApplicationDbContext _context;

    public SystemPromptRepository(ApplicationDbContext context) => _context = context;

    public async Task<SystemPrompt?> GetActivePromptAsync(int partId, int levelId, string purposeCode) =>
        await _context.SystemPrompts
            .AsNoTracking()
            .Include(x => x.Purpose)
            .Where(x => x.PartId == partId && x.LevelId == levelId && x.Purpose != null && x.Purpose.Code == purposeCode && x.IsActive)
            .FirstOrDefaultAsync();

    public async Task<List<SystemPrompt>> GetAllAsync() => await _context.SystemPrompts.ToListAsync();

    public async Task<SystemPrompt?> GetByIdAsync(int id) => await _context.SystemPrompts.FindAsync(id);

    public async Task<SystemPrompt> CreateAsync(SystemPrompt entity)
    {
        _context.SystemPrompts.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(SystemPrompt entity)
    {
        _context.SystemPrompts.Update(entity);
        await _context.SaveChangesAsync();
    }
}
