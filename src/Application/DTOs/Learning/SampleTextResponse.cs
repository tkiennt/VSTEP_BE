namespace Application.DTOs.Learning;

public class SampleTextResponse
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string SampleTypeCode { get; set; } = string.Empty;
}
