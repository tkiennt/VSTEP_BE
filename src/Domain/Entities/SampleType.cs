namespace Domain.Entities;

public class SampleType
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<SampleText> SampleTexts { get; set; } = new List<SampleText>();
}
