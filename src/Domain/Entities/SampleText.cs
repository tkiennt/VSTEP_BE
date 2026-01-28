namespace Domain.Entities;

public class SampleText
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public int LevelId { get; set; }
    public int SampleTypeId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Author { get; set; }

    // Navigation properties
    public virtual Topic? Topic { get; set; }
    public virtual Level? Level { get; set; }
    public virtual SampleType? SampleType { get; set; }
}
