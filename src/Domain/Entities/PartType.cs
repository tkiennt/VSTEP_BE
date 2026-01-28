using System.Collections.Generic;

namespace Domain.Entities;

public class PartType
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<Part> Parts { get; set; } = new List<Part>();
}
