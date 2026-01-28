namespace Application.DTOs.Learning;

public class HintResponse
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string HintTypeCode { get; set; } = string.Empty;
    public int? DisplayOrder { get; set; }
}
