using Application.DTOs.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using System.Security.Cryptography;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;

    public const int ResetTokenExpirationHours = 24;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IPasswordResetTokenRepository passwordResetTokenRepository)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordResetTokenRepository = passwordResetTokenRepository;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is deactivated");
        }

        var token = _jwtService.GenerateToken(user);
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var expiresAt = jwtToken.ValidTo;

        return new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsByUsernameAsync(request.Username))
        {
            throw new InvalidOperationException("Username already exists");
        }

        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        // Only allow Admin role to be set during registration if explicitly requested
        // Otherwise default to User role
        var role = request.Role == Role.Admin ? Role.User : request.Role;

        var user = new User
        {
            Name = string.IsNullOrWhiteSpace(request.Name) ? request.Username : request.Name,
            Username = request.Username,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = role,
            TargetLevelId = 1,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createdUser = await _userRepository.CreateAsync(user);
        var token = _jwtService.GenerateToken(createdUser);
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var expiresAt = jwtToken.ValidTo;

        return new AuthResponse
        {
            Token = token,
            Username = createdUser.Username,
            Email = createdUser.Email,
            Role = createdUser.Role,
            ExpiresAt = expiresAt
        };
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        return _jwtService.ValidateToken(token);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            return; // Don't reveal whether email exists

        var tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = tokenValue,
            ExpiresAt = DateTime.UtcNow.AddHours(ResetTokenExpirationHours),
            Used = false,
            CreatedAt = DateTime.UtcNow
        };
        await _passwordResetTokenRepository.CreateAsync(resetToken);
        // TODO: Send email with reset link containing tokenValue (e.g. https://yourapp.com/reset-password?token=...)
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var resetToken = await _passwordResetTokenRepository.GetByTokenAsync(request.Token);
        if (resetToken == null || resetToken.Used || resetToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired reset token");

        var user = await _userRepository.GetByIdAsync(resetToken.UserId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        resetToken.Used = true;
        await _passwordResetTokenRepository.UpdateAsync(resetToken);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
    }
}

