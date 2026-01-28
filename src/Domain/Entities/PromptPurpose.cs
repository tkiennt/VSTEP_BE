namespace Domain.Entities;

public class PromptPurpose
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<SystemPrompt> SystemPrompts { get; set; } = new List<SystemPrompt>();
}
