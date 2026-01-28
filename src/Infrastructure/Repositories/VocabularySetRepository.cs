using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VocabularySetRepository : IVocabularySetRepository
{
    private readonly ApplicationDbContext _context;

    public VocabularySetRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<VocabularySet>> GetByTopicAndLevelAsync(int topicId, int levelId) =>
        await _context.VocabularySets
            .AsNoTracking()
            .Include(x => x.VocabularyItems)
            .Include(x => x.SentenceStructures)
            .Where(x => x.TopicId == topicId && x.LevelId == levelId)
            .ToListAsync();

    public async Task<VocabularySet?> GetByIdAsync(int id) => 
        await _context.VocabularySets
            .Include(x => x.VocabularyItems)
            .Include(x => x.SentenceStructures)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<VocabularySet> CreateAsync(VocabularySet entity)
    {
        _context.VocabularySets.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<VocabularySet> UpdateAsync(VocabularySet entity)
    {
        _context.VocabularySets.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _context.VocabularySets.FindAsync(id);
        if (e != null) { _context.VocabularySets.Remove(e); await _context.SaveChangesAsync(); }
    }
}
