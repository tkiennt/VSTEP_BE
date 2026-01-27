using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PartTypeRepository : IPartTypeRepository
{
    private readonly ApplicationDbContext _context;

    public PartTypeRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<PartType>> GetAllAsync() =>
        await _context.PartTypes.AsNoTracking().OrderBy(x => x.Code).ToListAsync();

    public async Task<PartType?> GetByIdAsync(int id) =>
        await _context.PartTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
}
