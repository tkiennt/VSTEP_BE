using Application.DTOs.Evaluation;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class AIEvaluationService : IAIEvaluationService
{
    private readonly IUserSubmissionRepository _submissionRepository;
    private readonly IAIEvaluationRepository _evaluationRepository;
    private readonly ILanguageCheckRepository _languageCheckRepository;
    private readonly IScoringCriteriaRepository _criteriaRepository;

    public AIEvaluationService(
        IUserSubmissionRepository submissionRepository,
        IAIEvaluationRepository evaluationRepository,
        ILanguageCheckRepository languageCheckRepository,
        IScoringCriteriaRepository criteriaRepository)
    {
        _submissionRepository = submissionRepository;
        _evaluationRepository = evaluationRepository;
        _languageCheckRepository = languageCheckRepository;
        _criteriaRepository = criteriaRepository;
    }

    public async Task<EvaluationResponse> EvaluateSubmissionAsync(int submissionId)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId);
        if (submission == null) throw new InvalidOperationException("Submission not found");

        // 1. Language Check (Mock)
        var langCheck = new LanguageCheck
        {
            SubmissionId = submissionId,
            SpellingErrors = 2,
            GrammarErrors = 1,
            SyntaxErrors = 0,
            Feedback = "Good overall, but check minor spelling."
        };
        await _languageCheckRepository.CreateAsync(langCheck);

        // 2. AI Evaluation (Mock scoring based on criteria)
        var criteria = await _criteriaRepository.GetByPartIdAsync(submission.PartId);
        var evaluation = new AIEvaluation
        {
            SubmissionId = submissionId,
            OverallFeedback = "The essay follows the structure well.",
            Strengths = "Clear arguments and good vocabulary.",
            Weaknesses = "Conclusion could be stronger.",
            Suggestions = "Focus on linking words.",
            TotalScore = 0 // Will calculate
        };

        var criteriaScores = criteria.Select(c => new CriteriaScore
        {
            CriteriaId = c.Id,
            Score = 7.5f, // Mock score
            Feedback = $"Good performance in {c.Name}."
        }).ToList();

        evaluation.CriteriaScores = criteriaScores;
        evaluation.TotalScore = criteriaScores.Sum(cs => cs.Score ?? 0) / criteria.Count;

        await _evaluationRepository.CreateAsync(evaluation);

        return new EvaluationResponse
        {
            SubmissionId = submissionId,
            TotalScore = evaluation.TotalScore ?? 0,
            OverallFeedback = evaluation.OverallFeedback,
            Strengths = evaluation.Strengths,
            Weaknesses = evaluation.Weaknesses,
            Suggestions = evaluation.Suggestions,
            CriteriaScores = criteriaScores.Select(cs => new CriteriaScoreDto
            {
                CriteriaId = cs.CriteriaId,
                CriteriaName = criteria.First(c => c.Id == cs.CriteriaId).Name,
                Score = cs.Score ?? 0,
                Feedback = cs.Feedback
            }).ToList(),
            LanguageCheck = new LanguageCheckDto
            {
                SpellingErrors = langCheck.SpellingErrors,
                GrammarErrors = langCheck.GrammarErrors,
                SyntaxErrors = langCheck.SyntaxErrors,
                Feedback = langCheck.Feedback
            }
        };
    }
}
