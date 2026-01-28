namespace Domain.Entities;

public class UserSubmission
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int TopicId { get; set; }
    public int PartId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? WordCount { get; set; }
    public bool EnableHint { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual PracticeSession? Session { get; set; }
    public virtual Topic? Topic { get; set; }
    public virtual Part? Part { get; set; }
}
