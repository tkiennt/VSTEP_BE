namespace Application.DTOs.Dashboard;

public class CriteriaAvgScore
{
    public string CriteriaName { get; set; } = string.Empty;
    public float AverageScore { get; set; }
}

public class DashboardStatsResponse
{
    public string TargetLevel { get; set; } = string.Empty;
    public int TotalSubmissions { get; set; }
    public float OverallAverageScore { get; set; }
    public List<CriteriaAvgScore> CriteriaAverages { get; set; } = new();
    public List<string> RecommendedTopics { get; set; } = new();
}
