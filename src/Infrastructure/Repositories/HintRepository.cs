using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class HintRepository : IHintRepository
{
    private readonly ApplicationDbContext _context;

    public HintRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<Hint>> GetByTopicAndLevelAsync(int topicId, int levelId) =>
        await _context.Hints
            .AsNoTracking()
            .Include(x => x.HintType)
            .Where(x => x.TopicId == topicId && x.LevelId == levelId)
            .ToListAsync();

    public async Task<Hint?> GetByIdAsync(int id) => await _context.Hints.FindAsync(id);

    public async Task<Hint> CreateAsync(Hint entity)
    {
        _context.Hints.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Hint> UpdateAsync(Hint entity)
    {
        _context.Hints.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _context.Hints.FindAsync(id);
        if (e != null) { _context.Hints.Remove(e); await _context.SaveChangesAsync(); }
    }
}
