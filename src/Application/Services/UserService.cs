using Application.DTOs.User;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using BCrypt.Net;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPracticeSessionRepository _sessionRepository;
    private readonly IUserSubmissionRepository _submissionRepository;
    private readonly ILevelRepository _levelRepository;

    public UserService(
        IUserRepository userRepository,
        IPracticeSessionRepository sessionRepository,
        IUserSubmissionRepository submissionRepository,
        ILevelRepository levelRepository)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _submissionRepository = submissionRepository;
        _levelRepository = levelRepository;
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

    // Admin Management Methods
    
    public async Task<IEnumerable<UserListResponse>> GetAllUsersAsync(int page = 1, int pageSize = 10, string? search = null, string? sortBy = null, bool sortDesc = false)
    {
        var users = await _userRepository.GetAllAsync();
        
        // Apply search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            users = users.Where(u => 
                u.Name.ToLower().Contains(searchLower) ||
                u.Username.ToLower().Contains(searchLower) ||
                u.Email.ToLower().Contains(searchLower)).ToList();
        }
        
        // Apply sorting
        users = sortBy?.ToLower() switch
        {
            "name" => sortDesc ? users.OrderByDescending(u => u.Name).ToList() : users.OrderBy(u => u.Name).ToList(),
            "username" => sortDesc ? users.OrderByDescending(u => u.Username).ToList() : users.OrderBy(u => u.Username).ToList(),
            "email" => sortDesc ? users.OrderByDescending(u => u.Email).ToList() : users.OrderBy(u => u.Email).ToList(),
            "role" => sortDesc ? users.OrderByDescending(u => u.Role).ToList() : users.OrderBy(u => u.Role).ToList(),
            "createdat" => sortDesc ? users.OrderByDescending(u => u.CreatedAt).ToList() : users.OrderBy(u => u.CreatedAt).ToList(),
            _ => sortDesc ? users.OrderByDescending(u => u.Id).ToList() : users.OrderBy(u => u.Id).ToList()
        };
        
        // Apply pagination
        var paginatedUsers = users.Skip((page - 1) * pageSize).Take(pageSize);
        
        // Map to response DTOs
        var responses = new List<UserListResponse>();
        foreach (var user in paginatedUsers)
        {
            var sessions = await _sessionRepository.GetByUserIdAsync(user.Id);
            var targetLevel = user.TargetLevelId.HasValue ? await _levelRepository.GetByIdAsync(user.TargetLevelId.Value) : null;
            
            responses.Add(new UserListResponse
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                TargetLevelName = targetLevel?.LevelCode,
                PracticeSessionCount = sessions.Count,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive
            });
        }
        
        return responses;
    }

    public async Task<AdminUserResponse?> GetUserByIdForAdminAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;
        
        var sessions = await _sessionRepository.GetByUserIdAsync(userId);
        var submissions = await _submissionRepository.GetByUserIdAsync(userId);
        var targetLevel = user.TargetLevelId.HasValue ? await _levelRepository.GetByIdAsync(user.TargetLevelId.Value) : null;
        
        return new AdminUserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            RoleName = user.Role.ToString(),
            TargetLevelId = user.TargetLevelId,
            TargetLevelName = targetLevel?.LevelCode,
            PracticeSessionCount = sessions.Count,
            SubmissionCount = submissions.Count,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive,
            LastLoginAt = null // TODO: Implement last login tracking
        };
    }

    public async Task<AdminUserResponse> CreateUserAsync(CreateUserRequest request)
    {
        // Validate uniqueness
        if (await _userRepository.ExistsByUsernameAsync(request.Username))
            throw new InvalidOperationException("Username already exists");
        
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            throw new InvalidOperationException("Email already exists");
        
        var user = new User
        {
            Name = request.Name,
            Username = request.Username,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            TargetLevelId = request.TargetLevelId,
            IsActive = request.IsActive
        };
        
        var createdUser = await _userRepository.CreateAsync(user);
        
        var targetLevel = createdUser.TargetLevelId.HasValue ? await _levelRepository.GetByIdAsync(createdUser.TargetLevelId.Value) : null;
        
        return new AdminUserResponse
        {
            Id = createdUser.Id,
            Name = createdUser.Name,
            Username = createdUser.Username,
            Email = createdUser.Email,
            PhoneNumber = createdUser.PhoneNumber,
            Role = createdUser.Role,
            RoleName = createdUser.Role.ToString(),
            TargetLevelId = createdUser.TargetLevelId,
            TargetLevelName = targetLevel?.LevelCode,
            PracticeSessionCount = 0,
            SubmissionCount = 0,
            CreatedAt = createdUser.CreatedAt,
            UpdatedAt = createdUser.UpdatedAt,
            IsActive = createdUser.IsActive,
            LastLoginAt = null
        };
    }

    public async Task<AdminUserResponse?> UpdateUserAsync(int userId, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;
        
        user.Name = request.Name;
        user.PhoneNumber = request.PhoneNumber;
        user.Role = request.Role;
        user.TargetLevelId = request.TargetLevelId;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        
        var updatedUser = await _userRepository.UpdateAsync(user);
        
        var sessions = await _sessionRepository.GetByUserIdAsync(userId);
        var submissions = await _submissionRepository.GetByUserIdAsync(userId);
        var targetLevel = updatedUser.TargetLevelId.HasValue ? await _levelRepository.GetByIdAsync(updatedUser.TargetLevelId.Value) : null;
        
        return new AdminUserResponse
        {
            Id = updatedUser.Id,
            Name = updatedUser.Name,
            Username = updatedUser.Username,
            Email = updatedUser.Email,
            PhoneNumber = updatedUser.PhoneNumber,
            Role = updatedUser.Role,
            RoleName = updatedUser.Role.ToString(),
            TargetLevelId = updatedUser.TargetLevelId,
            TargetLevelName = targetLevel?.LevelCode,
            PracticeSessionCount = sessions.Count,
            SubmissionCount = submissions.Count,
            CreatedAt = updatedUser.CreatedAt,
            UpdatedAt = updatedUser.UpdatedAt,
            IsActive = updatedUser.IsActive,
            LastLoginAt = null
        };
    }

    public async Task<bool> UpdateUserPasswordAsync(int userId, UpdateUserPasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;
        
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;
        
        // Don't allow deletion of admin users
        if (user.Role == Role.Admin)
            throw new InvalidOperationException("Cannot delete admin users");
        
        await _userRepository.DeleteAsync(userId);
        return true;
    }

    public async Task<bool> ToggleUserStatusAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;
        
        // Don't allow toggling admin status
        if (user.Role == Role.Admin)
            throw new InvalidOperationException("Cannot change admin user status");
        
        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<int> GetTotalUserCountAsync()
    {
        return await _userRepository.GetTotalCountAsync();
    }
}
