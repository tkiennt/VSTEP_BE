using System.Collections.Generic;

namespace Domain.Entities;

public class Level
{
    public int Id { get; set; }
    public string LevelCode { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
