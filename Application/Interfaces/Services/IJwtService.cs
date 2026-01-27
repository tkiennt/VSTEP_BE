using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
    string? GetUsernameFromToken(string token);
}

