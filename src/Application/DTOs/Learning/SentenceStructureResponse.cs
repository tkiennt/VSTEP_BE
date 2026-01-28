namespace Application.DTOs.Learning;

public class SentenceStructureResponse
{
    public int Id { get; set; }
    public string Pattern { get; set; } = string.Empty;
    public string? UsageNote { get; set; }
    public string? Example { get; set; }
}
