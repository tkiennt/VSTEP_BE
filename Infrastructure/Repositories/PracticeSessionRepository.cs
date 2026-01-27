using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PracticeSessionRepository : IPracticeSessionRepository
{
    private readonly ApplicationDbContext _context;

    public PracticeSessionRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<PracticeSession>> GetByUserIdAsync(int userId) =>
        await _context.PracticeSessions.AsNoTracking().Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).ToListAsync();

    public async Task<PracticeSession?> GetByIdAsync(int id) => await _context.PracticeSessions.FindAsync(id);

    public async Task<PracticeSession> CreateAsync(PracticeSession entity)
    {
        _context.PracticeSessions.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
