using Application.DTOs.User;

namespace Application.Interfaces.Services;

public interface IUserService
{
    // User profile management (for authenticated users)
    Task<UserProfileResponse?> GetProfileByUsernameAsync(string username);
    Task<UserProfileResponse?> GetProfileByIdAsync(int userId);
    Task<UserProfileResponse?> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    
    // Admin user management
    Task<IEnumerable<UserListResponse>> GetAllUsersAsync(int page = 1, int pageSize = 10, string? search = null, string? sortBy = null, bool sortDesc = false);
    Task<AdminUserResponse?> GetUserByIdForAdminAsync(int userId);
    Task<AdminUserResponse> CreateUserAsync(CreateUserRequest request);
    Task<AdminUserResponse?> UpdateUserAsync(int userId, UpdateUserRequest request);
    Task<bool> UpdateUserPasswordAsync(int userId, UpdateUserPasswordRequest request);
    Task<bool> DeleteUserAsync(int userId);
    Task<bool> ToggleUserStatusAsync(int userId);
    Task<int> GetTotalUserCountAsync();
}
