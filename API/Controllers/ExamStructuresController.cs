using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/exam-structures")]
public class ExamStructuresController : ControllerBase
{
    private readonly IExamStructureRepository _repo;

    public ExamStructuresController(IExamStructureRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateExamStructureRequest request)
    {
        var entity = new ExamStructure
        {
            Name = request.Name,
            TotalParts = request.TotalParts,
            Description = request.Description
        };
        var created = await _repo.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateExamStructureRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();
        existing.Name = request.Name;
        existing.TotalParts = request.TotalParts;
        existing.Description = request.Description;
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

public class CreateExamStructureRequest
{
    public string Name { get; set; } = string.Empty;
    public int TotalParts { get; set; }
    public string? Description { get; set; }
}
