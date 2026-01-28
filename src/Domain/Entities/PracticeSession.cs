namespace Domain.Entities;

public class PracticeSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ModeId { get; set; }
    public bool IsRandom { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual PracticeMode? Mode { get; set; }
    public virtual ICollection<UserSubmission> UserSubmissions { get; set; } = new List<UserSubmission>();
}
