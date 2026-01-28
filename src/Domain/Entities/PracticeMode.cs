using System.Collections.Generic;

namespace Domain.Entities;

public class PracticeMode
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<PracticeSession> PracticeSessions { get; set; } = new List<PracticeSession>();
}
