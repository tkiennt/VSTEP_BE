using Application.DTOs.User;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<UserProfileResponse?> GetProfileByUsernameAsync(string username);
    Task<UserProfileResponse?> GetProfileByIdAsync(int userId);
    Task<UserProfileResponse?> UpdateProfileAsync(int userId, UpdateProfileRequest request);
}
