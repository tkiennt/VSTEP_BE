namespace Domain.Entities;

public class VocabularySet
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public int LevelId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual Topic? Topic { get; set; }
    public virtual Level? Level { get; set; }
    public virtual ICollection<VocabularyItem> VocabularyItems { get; set; } = new List<VocabularyItem>();
    public virtual ICollection<SentenceStructure> SentenceStructures { get; set; } = new List<SentenceStructure>();
}
