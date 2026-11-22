using AutoMapper;
using Microsoft.Extensions.Logging;
using RealEstateAPI.Application.DTOs.Auth;
using RealEstateAPI.Application.Interfaces;
using RealEstateAPI.Domain.Entities;
using RealEstateAPI.Infrastructure.Repositories.Interfaces;
using RealEstateAPI.Infrastructure.Identity;
using RealEstateAPI.Domain.Enums;

namespace RealEstateAPI.Application.Services;

/// <summary>
/// Authentication service handling user registration and login
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IMapper mapper,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists", request.Email);
                return null;
            }

            // Create user entity
            var user = _mapper.Map<User>(request);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.Role = UserRole.User; // Default role
            user.IsActive = true;

            // Save to database
            var createdUser = await _userRepository.AddAsync(user);
            
            // Generate JWT token
            var token = _jwtTokenGenerator.GenerateToken(createdUser);
            
            var response = _mapper.Map<AuthResponseDto>(createdUser);
            response.Token = token;
            response.ExpiresAt = DateTime.UtcNow.AddHours(24);

            _logger.LogInformation("User registered successfully: {Email}", request.Email);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        try
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
                return null;
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid password for email {Email}", request.Email);
                return null;
            }

            // Check if user is active
            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed: User account is inactive for email {Email}", request.Email);
                return null;
            }

            // Generate JWT token
            var token = _jwtTokenGenerator.GenerateToken(user);
            
            var response = _mapper.Map<AuthResponseDto>(user);
            response.Token = token;
            response.ExpiresAt = DateTime.UtcNow.AddHours(24);

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            throw;
        }
    }
}
