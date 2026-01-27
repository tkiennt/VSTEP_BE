using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/user-submissions")]
[Authorize]
public class UserSubmissionsController : ControllerBase
{
    private readonly IUserSubmissionRepository _repo;
    private readonly IPracticeSessionRepository _sessionRepo;

    public UserSubmissionsController(IUserSubmissionRepository repo, IPracticeSessionRepository sessionRepo)
    {
        _repo = repo;
        _sessionRepo = sessionRepo;
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }

    [HttpGet("by-session/{sessionId:int}")]
    public async Task<IActionResult> GetBySession(int sessionId)
    {
        var session = await _sessionRepo.GetByIdAsync(sessionId);
        if (session == null) return NotFound();
        var userId = GetCurrentUserId();
        if (userId != session.UserId) return Forbid();
        return Ok(await _repo.GetBySessionIdAsync(sessionId));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return NotFound();
        var session = await _sessionRepo.GetByIdAsync(item.SessionId);
        if (session == null) return NotFound();
        var userId = GetCurrentUserId();
        if (userId != session.UserId) return Forbid();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserSubmissionRequest request)
    {
        var session = await _sessionRepo.GetByIdAsync(request.SessionId);
        if (session == null) return NotFound("Session not found");
        var userId = GetCurrentUserId();
        if (userId != session.UserId) return Forbid();
        var entity = new UserSubmission
        {
            SessionId = request.SessionId,
            TopicId = request.TopicId,
            PartId = request.PartId,
            Content = request.Content,
            WordCount = request.WordCount ?? request.Content.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length,
            EnableHint = request.EnableHint
        };
        var created = await _repo.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}

public class CreateUserSubmissionRequest
{
    public int SessionId { get; set; }
    public int TopicId { get; set; }
    public int PartId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? WordCount { get; set; }
    public bool EnableHint { get; set; }
}
