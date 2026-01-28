namespace Domain.Entities;

public class Part
{
    public int Id { get; set; }
    public int ExamStructureId { get; set; }
    public int PartTypeId { get; set; }
    public string? Description { get; set; }
    public int? TimeLimit { get; set; }
    public int? MinWords { get; set; }
    public int? MaxWords { get; set; }

    // Navigation properties
    public virtual ExamStructure? ExamStructure { get; set; }
    public virtual PartType? PartType { get; set; }
    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
    public virtual ICollection<UserSubmission> UserSubmissions { get; set; } = new List<UserSubmission>();
    public virtual ICollection<ScoringCriteria> ScoringCriteria { get; set; } = new List<ScoringCriteria>();
    public virtual ICollection<SystemPrompt> SystemPrompts { get; set; } = new List<SystemPrompt>();
}
