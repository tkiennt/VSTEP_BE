namespace Domain.Entities;

public class Part
{
    public int Id { get; set; }
    public int ExamStructureId { get; set; }
    public int PartTypeId { get; set; }
    public string? Description { get; set; }
    public int? TimeLimit { get; set; }
    public int? MinWords { get; set; }
    public int? MaxWords { get; set; }
}
