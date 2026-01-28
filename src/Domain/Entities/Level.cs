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
    public virtual ICollection<VocabularySet> VocabularySets { get; set; } = new List<VocabularySet>();
    public virtual ICollection<SampleText> SampleTexts { get; set; } = new List<SampleText>();
    public virtual ICollection<Hint> Hints { get; set; } = new List<Hint>();
    public virtual ICollection<AIEvaluation> AIEvaluations { get; set; } = new List<AIEvaluation>();
    public virtual ICollection<SystemPrompt> SystemPrompts { get; set; } = new List<SystemPrompt>();
}
