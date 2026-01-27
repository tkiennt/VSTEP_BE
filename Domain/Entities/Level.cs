namespace Domain.Entities;

public class Level
{
    public int Id { get; set; }
    public string LevelCode { get; set; } = string.Empty;
    public string? Description { get; set; }
}
