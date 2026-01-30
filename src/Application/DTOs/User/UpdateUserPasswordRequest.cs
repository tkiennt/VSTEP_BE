namespace Application.DTOs.User;

/// <summary>
/// Request DTO for updating user password (admin only)
/// </summary>
public class UpdateUserPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}