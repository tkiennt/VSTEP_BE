using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly ApplicationDbContext _context;

    public TopicRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<Topic>> GetByPartIdAsync(int partId) =>
        await _context.Topics.AsNoTracking().Where(x => x.PartId == partId).ToListAsync();

    public async Task<Topic?> GetByIdAsync(int id) => await _context.Topics.FindAsync(id);

    public async Task<Topic> CreateAsync(Topic entity)
    {
        _context.Topics.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Topic> UpdateAsync(Topic entity)
    {
        _context.Topics.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _context.Topics.FindAsync(id);
        if (e != null) { _context.Topics.Remove(e); await _context.SaveChangesAsync(); }
    }
}
