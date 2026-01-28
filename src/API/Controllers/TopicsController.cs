using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/topics")]
public class TopicsController : ControllerBase
{
    private readonly ITopicRepository _repo;

    public TopicsController(ITopicRepository repo) => _repo = repo;

    [HttpGet("by-part/{partId:int}")]
    public async Task<IActionResult> GetByPart(int partId) =>
        Ok(await _repo.GetByPartIdAsync(partId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateTopicRequest request)
    {
        var entity = new Topic
        {
            PartId = request.PartId,
            TopicName = request.TopicName,
            Context = request.Context,
            Purpose = request.Purpose,
            RecipientRole = request.RecipientRole,
            DifficultyLevelId = request.DifficultyLevelId
        };
        var created = await _repo.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateTopicRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();
        existing.PartId = request.PartId;
        existing.TopicName = request.TopicName;
        existing.Context = request.Context;
        existing.Purpose = request.Purpose;
        existing.RecipientRole = request.RecipientRole;
        existing.DifficultyLevelId = request.DifficultyLevelId;
        await _repo.UpdateAsync(existing);
        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}

public class CreateTopicRequest
{
    public int PartId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public string? Context { get; set; }
    public string? Purpose { get; set; }
    public string? RecipientRole { get; set; }
    public int DifficultyLevelId { get; set; }
}
