namespace Domain.Entities;

public class CriteriaScore
{
    public int Id { get; set; }
    public int EvaluationId { get; set; }
    public int CriteriaId { get; set; }
    public float? Score { get; set; }
    public string? Feedback { get; set; }

    // Navigation properties
    public virtual AIEvaluation? Evaluation { get; set; }
    public virtual ScoringCriteria? Criteria { get; set; }
}
