using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SampleTextRepository : ISampleTextRepository
{
    private readonly ApplicationDbContext _context;

    public SampleTextRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<SampleText>> GetByTopicAndLevelAsync(int topicId, int levelId) =>
        await _context.SampleTexts
            .AsNoTracking()
            .Include(x => x.SampleType)
            .Where(x => x.TopicId == topicId && x.LevelId == levelId)
            .ToListAsync();

    public async Task<SampleText?> GetByIdAsync(int id) => await _context.SampleTexts.FindAsync(id);

    public async Task<SampleText> CreateAsync(SampleText entity)
    {
        _context.SampleTexts.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<SampleText> UpdateAsync(SampleText entity)
    {
        _context.SampleTexts.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _context.SampleTexts.FindAsync(id);
        if (e != null) { _context.SampleTexts.Remove(e); await _context.SaveChangesAsync(); }
    }
}
