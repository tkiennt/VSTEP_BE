using Domain.Enums;

namespace Application.DTOs.User;

/// <summary>
/// Response DTO for listing users (used in admin endpoints)
/// </summary>
public class UserListResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Role Role { get; set; }
    public string? TargetLevelName { get; set; }
    public int PracticeSessionCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}