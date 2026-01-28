using Application.DTOs.Dashboard;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserSubmissionRepository _submissionRepository;
    private readonly ITopicRepository _topicRepository;

    public DashboardService(
        IUserRepository userRepository,
        IUserSubmissionRepository submissionRepository,
        ITopicRepository topicRepository)
    {
        _userRepository = userRepository;
        _submissionRepository = submissionRepository;
        _topicRepository = topicRepository;
    }

    public async Task<DashboardStatsResponse> GetUserStatsAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new InvalidOperationException("User not found");

        var submissions = await _submissionRepository.GetByUserIdAsync(userId);
        var evaluations = submissions
            .Where(s => s.AIEvaluation != null)
            .Select(s => s.AIEvaluation!)
            .ToList();

        var criteriaScores = evaluations
            .SelectMany(e => e.CriteriaScores)
            .GroupBy(cs => cs.Criteria?.Name ?? "Unknown")
            .Select(g => new CriteriaAvgScore
            {
                CriteriaName = g.Key,
                AverageScore = g.Average(cs => cs.Score ?? 0)
            })
            .ToList();

        var overallAvg = evaluations.Any() ? evaluations.Average(e => e.TotalScore ?? 0) : 0;

        // Simple recommendation: topics in target level not yet submitted
        var submittedTopicIds = submissions.Select(s => s.TopicId).Distinct().ToList();
        var allTopicsInTargetLevel = await _topicRepository.GetByPartIdAsync(1); // Placeholder: should filter by level
        // For now, just getting some topics
        var recommended = allTopicsInTargetLevel
            .Where(t => !submittedTopicIds.Contains(t.Id))
            .Take(3)
            .Select(t => t.TopicName)
            .ToList();

        return new DashboardStatsResponse
        {
            TargetLevel = user.TargetLevel?.LevelCode ?? "N/A",
            TotalSubmissions = submissions.Count,
            OverallAverageScore = (float)overallAvg,
            CriteriaAverages = criteriaScores,
            RecommendedTopics = recommended
        };
    }
}
