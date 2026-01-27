namespace Domain.Entities;

public class PracticeSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ModeId { get; set; }
    public bool IsRandom { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
