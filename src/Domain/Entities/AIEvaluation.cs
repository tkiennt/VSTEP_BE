namespace Domain.Entities;

public class AIEvaluation
{
    public int Id { get; set; }
    public int SubmissionId { get; set; }
    public float? TotalScore { get; set; }
    public int? EstimatedLevelId { get; set; }
    public string? OverallFeedback { get; set; }
    public string? Strengths { get; set; }
    public string? Weaknesses { get; set; }
    public string? Suggestions { get; set; }
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual UserSubmission? Submission { get; set; }
    public virtual Level? EstimatedLevel { get; set; }
    public virtual ICollection<CriteriaScore> CriteriaScores { get; set; } = new List<CriteriaScore>();
}
