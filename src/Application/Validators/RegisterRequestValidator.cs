using Application.DTOs.Auth;
using Application.Interfaces.Repositories;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validators;

/// <summary>
/// Validator for user registration requests with comprehensive validation rules
/// Includes async database validation for username and email uniqueness
/// </summary>
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private readonly IUserRepository _userRepository;

    public RegisterRequestValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        // Name Validation
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
            .Matches(@"^[\p{L}\s'-]+$").WithMessage("Name can only contain letters, spaces, hyphens, and apostrophes.");

        // Username Validation
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.")
            .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Username can only contain letters, numbers, underscores, and hyphens.")
            .Must(username => !username.StartsWith("_") && !username.EndsWith("_"))
                .WithMessage("Username cannot start or end with an underscore.")
            .Must(username => !username.Contains("__"))
                .WithMessage("Username cannot contain consecutive underscores.")
            .MustAsync(async (username, cancellation) => !await _userRepository.ExistsByUsernameAsync(username))
                .WithMessage("Username '{PropertyValue}' is already taken. Please choose a different username.");

        // Email Validation
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.")
            .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.")
            .Must(BeValidEmailDomain).WithMessage("Email domain is not allowed.")
            .MustAsync(async (email, cancellation) => !await _userRepository.ExistsByEmailAsync(email))
                .WithMessage("Email '{PropertyValue}' is already registered. Please use a different email address.");

        // Phone Number Validation (Vietnamese format)
        RuleFor(x => x.PhoneNumber)
            .Must(BeValidVietnamesePhoneNumber)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("Phone number must be a valid Vietnamese phone number (e.g., 0912345678 or +84912345678).");

        // Password Validation
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]").WithMessage("Password must contain at least one special character (!@#$%^&*()_+=[]{};<>|./?,-).");
    }

    /// <summary>
    /// Validates Vietnamese phone number formats
    /// Supports: 
    /// - 10 digits starting with 0 (e.g., 0912345678)
    /// - International format with +84 (e.g., +84912345678)
    /// - International format with 84 (e.g., 84912345678)
    /// </summary>
    private bool BeValidVietnamesePhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return true; // Optional field

        // Remove spaces and dashes
        var cleanNumber = Regex.Replace(phoneNumber, @"[\s-]", "");

        // Vietnamese phone number patterns
        var patterns = new[]
        {
            @"^0[3|5|7|8|9][0-9]{8}$",           // 10 digits starting with 0 (e.g., 0912345678)
            @"^\+84[3|5|7|8|9][0-9]{8}$",        // +84 format (e.g., +84912345678)
            @"^84[3|5|7|8|9][0-9]{8}$"           // 84 format (e.g., 84912345678)
        };

        return patterns.Any(pattern => Regex.IsMatch(cleanNumber, pattern));
    }

    /// <summary>
    /// Validates email domain (optional: can implement whitelist/blacklist)
    /// </summary>
    private bool BeValidEmailDomain(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Optional: Add blacklisted domains
        var blacklistedDomains = new[] { "tempmail.com", "throwaway.email", "guerrillamail.com" };
        
        var domain = email.Split('@').LastOrDefault()?.ToLower();
        
        if (string.IsNullOrEmpty(domain))
            return false;

        return !blacklistedDomains.Contains(domain);
    }
}
