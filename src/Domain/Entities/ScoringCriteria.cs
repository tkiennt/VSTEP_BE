namespace Domain.Entities;

public class ScoringCriteria
{
    public int Id { get; set; }
    public int PartId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public float Weight { get; set; }
    public int MaxScore { get; set; } = 10;

    // Navigation properties
    public virtual Part? Part { get; set; }
    public virtual ICollection<CriteriaScore> CriteriaScores { get; set; } = new List<CriteriaScore>();
}
