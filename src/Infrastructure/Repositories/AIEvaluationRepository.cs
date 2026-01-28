using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AIEvaluationRepository : IAIEvaluationRepository
{
    private readonly ApplicationDbContext _context;

    public AIEvaluationRepository(ApplicationDbContext context) => _context = context;

    public async Task<AIEvaluation?> GetBySubmissionIdAsync(int submissionId) =>
        await _context.AIEvaluations
            .Include(x => x.CriteriaScores)
            .ThenInclude(x => x.Criteria)
            .FirstOrDefaultAsync(x => x.SubmissionId == submissionId);

    public async Task<AIEvaluation?> GetByIdAsync(int id) => 
        await _context.AIEvaluations
            .Include(x => x.CriteriaScores)
            .ThenInclude(x => x.Criteria)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<AIEvaluation> CreateAsync(AIEvaluation entity)
    {
        _context.AIEvaluations.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<AIEvaluation> UpdateAsync(AIEvaluation entity)
    {
        _context.AIEvaluations.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
