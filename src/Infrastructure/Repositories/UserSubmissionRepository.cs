using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserSubmissionRepository : IUserSubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public UserSubmissionRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<UserSubmission>> GetBySessionIdAsync(int sessionId) =>
        await _context.UserSubmissions.AsNoTracking().Where(x => x.SessionId == sessionId).OrderByDescending(x => x.SubmittedAt).ToListAsync();

    public async Task<List<UserSubmission>> GetByUserIdAsync(int userId) =>
        await _context.UserSubmissions
            .AsNoTracking()
            .Include(x => x.Session)
            .Include(x => x.AIEvaluation)
                .ThenInclude(e => e!.CriteriaScores)
                    .ThenInclude(cs => cs.Criteria)
            .Where(x => x.Session != null && x.Session.UserId == userId)
            .OrderByDescending(x => x.SubmittedAt)
            .ToListAsync();

    public async Task<UserSubmission?> GetByIdAsync(int id) => await _context.UserSubmissions.FindAsync(id);

    public async Task<UserSubmission> CreateAsync(UserSubmission entity)
    {
        _context.UserSubmissions.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
