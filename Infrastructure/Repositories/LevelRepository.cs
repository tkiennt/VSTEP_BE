using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LevelRepository : ILevelRepository
{
    private readonly ApplicationDbContext _context;

    public LevelRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<Level>> GetAllAsync() =>
        await _context.Levels.AsNoTracking().OrderBy(x => x.LevelCode).ToListAsync();

    public async Task<Level?> GetByIdAsync(int id) =>
        await _context.Levels.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
}
