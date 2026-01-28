using Application.DTOs.Learning;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Application.Services;

public class HintService : IHintService
{
    private readonly IHintRepository _hintRepository;   

    public HintService(IHintRepository hintRepository)
    {
        _hintRepository = hintRepository;
    }

    public async Task<List<HintResponse>> GetHintsAsync(int topicId, int levelId)
    {
        var hints = await _hintRepository.GetByTopicAndLevelAsync(topicId, levelId);
        return hints.Select(h => new HintResponse
        {
            Id = h.Id,
            Content = h.Content,
            HintTypeCode = h.HintType?.Code ?? string.Empty,
            DisplayOrder = h.DisplayOrder
        }).ToList();
    }
}
