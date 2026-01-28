namespace Domain.Entities;

public class SystemPrompt
{
    public int Id { get; set; }
    public int PartId { get; set; }
    public int LevelId { get; set; }
    public int PurposeId { get; set; }
    public string PromptContent { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Part? Part { get; set; }
    public virtual Level? Level { get; set; }
    public virtual PromptPurpose? Purpose { get; set; }
}
