namespace Domain.Entities;

public class Hint
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public int LevelId { get; set; }
    public int HintTypeId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? DisplayOrder { get; set; }

    // Navigation properties
    public virtual Topic? Topic { get; set; }
    public virtual Level? Level { get; set; }
    public virtual HintType? HintType { get; set; }
}
