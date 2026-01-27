using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/practice-sessions")]
[Authorize]
public class PracticeSessionsController : ControllerBase
{
    private readonly IPracticeSessionRepository _repo;

    public PracticeSessionsController(IPracticeSessionRepository repo) => _repo = repo;

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMySessions()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();
        return Ok(await _repo.GetByUserIdAsync(userId.Value));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return NotFound();
        var userId = GetCurrentUserId();
        if (userId != item.UserId) return Forbid();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePracticeSessionRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();
        var entity = new PracticeSession
        {
            UserId = userId.Value,
            ModeId = request.ModeId,
            IsRandom = request.IsRandom
        };
        var created = await _repo.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}

public class CreatePracticeSessionRequest
{
    public int ModeId { get; set; }
    public bool IsRandom { get; set; } = true;
}
