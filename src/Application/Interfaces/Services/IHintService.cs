using Application.DTOs.Learning;

namespace Application.Interfaces.Services;

public interface IHintService
{
    Task<List<HintResponse>> GetHintsAsync(int topicId, int levelId);
}
