using System.Collections.Generic;

namespace Domain.Entities;

public class ExamStructure
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalParts { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<Part> Parts { get; set; } = new List<Part>();
}
