using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/parts")]
public class PartsController : ControllerBase
{
    private readonly IPartRepository _repo;

    public PartsController(IPartRepository repo) => _repo = repo;

    [HttpGet("by-exam/{examStructureId:int}")]
    public async Task<IActionResult> GetByExamStructure(int examStructureId) =>
        Ok(await _repo.GetByExamStructureIdAsync(examStructureId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Create([FromBody] CreatePartRequest request)
    {
        var entity = new Part
        {
            ExamStructureId = request.ExamStructureId,
            PartTypeId = request.PartTypeId,
            Description = request.Description,
            TimeLimit = request.TimeLimit,
            MinWords = request.MinWords,
            MaxWords = request.MaxWords
        };
        var created = await _repo.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePartRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();
        existing.ExamStructureId = request.ExamStructureId;
        existing.PartTypeId = request.PartTypeId;
        existing.Description = request.Description;
        existing.TimeLimit = request.TimeLimit;
        existing.MinWords = request.MinWords;
        existing.MaxWords = request.MaxWords;
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

public class CreatePartRequest
{
    public int ExamStructureId { get; set; }
    public int PartTypeId { get; set; }
    public string? Description { get; set; }
    public int? TimeLimit { get; set; }
    public int? MinWords { get; set; }
    public int? MaxWords { get; set; }
}
