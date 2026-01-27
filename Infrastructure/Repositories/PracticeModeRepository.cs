using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PracticeModeRepository : IPracticeModeRepository
{
    private readonly ApplicationDbContext _context;

    public PracticeModeRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<PracticeMode>> GetAllAsync() =>
        await _context.PracticeModes.AsNoTracking().OrderBy(x => x.Code).ToListAsync();

    public async Task<PracticeMode?> GetByIdAsync(int id) =>
        await _context.PracticeModes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
}
