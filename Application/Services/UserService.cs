using Application.DTOs.User;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileResponse?> GetProfileByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user == null ? null : MapToProfile(user);
    }

    public async Task<UserProfileResponse?> GetProfileByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user == null ? null : MapToProfile(user);
    }

    public async Task<UserProfileResponse?> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        if (request.Email != user.Email && await _userRepository.ExistsByEmailAsync(request.Email))
            throw new InvalidOperationException("Email already in use");

        user.Name = request.Name;
        user.Email = request.Email;
        user.TargetLevelId = request.TargetLevelId;
        user.UpdatedAt = DateTime.UtcNow;
        var updated = await _userRepository.UpdateAsync(user);
        return MapToProfile(updated);
    }

    private static UserProfileResponse MapToProfile(User user)
    {
        return new UserProfileResponse
        {
            Id = user.Id,
            Name = user.Name,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            TargetLevelId = user.TargetLevelId,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive
        };
    }
}
