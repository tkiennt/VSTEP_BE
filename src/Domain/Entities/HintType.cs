namespace Domain.Entities;

public class HintType
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<Hint> Hints { get; set; } = new List<Hint>();
}
