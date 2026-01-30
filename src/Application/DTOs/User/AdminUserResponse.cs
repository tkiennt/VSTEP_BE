using Domain.Enums;

namespace Application.DTOs.User;

/// <summary>
/// Detailed user response for admin management
/// </summary>
public class AdminUserResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Role Role { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int? TargetLevelId { get; set; }
    public string? TargetLevelName { get; set; }
    public int PracticeSessionCount { get; set; }
    public int SubmissionCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
}