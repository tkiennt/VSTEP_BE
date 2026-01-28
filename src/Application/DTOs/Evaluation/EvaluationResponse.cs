namespace Application.DTOs.Evaluation;

public class CriteriaScoreDto
{
    public int CriteriaId { get; set; }
    public string CriteriaName { get; set; } = string.Empty;
    public float Score { get; set; }
    public string? Feedback { get; set; }
}

public class EvaluationResponse
{
    public int SubmissionId { get; set; }
    public float TotalScore { get; set; }
    public string? EstimatedLevel { get; set; }
    public string? OverallFeedback { get; set; }
    public string? Strengths { get; set; }
    public string? Weaknesses { get; set; }
    public string? Suggestions { get; set; }
    public List<CriteriaScoreDto> CriteriaScores { get; set; } = new();
    public LanguageCheckDto? LanguageCheck { get; set; }
}

public class LanguageCheckDto
{
    public int SpellingErrors { get; set; }
    public int GrammarErrors { get; set; }
    public int SyntaxErrors { get; set; }
    public string? Feedback { get; set; }
}
