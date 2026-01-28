using Azure.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using VotingPoll.Core.Cryptography;
using VotingPoll.Core.Entities.Authentication;
using VotingPoll.Core.Exceptions.AuthExceptions;
using VotingPoll.Core.Interfaces.Authentication;
using VotingPoll.Core.Interfaces.Repositories.Authentication;
using VotingPoll.Core.Interfaces.ServicesInterfaces.Authentication;
using VotingPoll.Core.Models.DTOs.Authentication;
using EmailSvc = VotingPoll.Core.Services.EmailService.EmailService;

namespace VotingPoll.Core.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthService> _logger;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly EmailSvc _emailService;

    public AuthService(ITokenService tokenService, IUserRepository userRepository, ILogger<AuthService> logger,
        IRefreshTokenRepository refreshTokenRepository, EmailSvc emailService)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
        _emailService = emailService;
    }

    public async Task<AuthDto.AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (_userRepository.ExistsAsync(request.Email).Result)
            throw new EmailAlreadyExistsException($"{request.Email}");

        byte[] salt = CryptographyHelper.GenerateSalt(16);
        byte[] hashedPassword = CryptographyHelper.HashPassword(request.Password, salt);

        string verificationToken = Guid.NewGuid().ToString();

        User createdUser = new User
        {
            Id = 0,
            Email = request.Email,
            PasswordHash = hashedPassword,
            Role = Role.User,
            CreatedAt = DateTime.UtcNow,
            RefreshTokens = new List<RefreshToken>(),
            IsVerified = false,
            VerificationToken = verificationToken,
            VerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24),
        };

        createdUser.VerificationEmailLastSentAt = DateTime.UtcNow;
        await _userRepository.CreateUserAsync(createdUser);
        _logger.LogInformation($"Created user with id: {createdUser.Id}");

        try
        {
            await _emailService.SendVerificationEmailAsync(createdUser.Email, verificationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send verification email to {createdUser.Email}");
        }

        // Return empty tokens — user must verify email before logging in
        return new AuthDto.AuthResponse { AccessToken = null, RefreshToken = null };
    }

    public async Task<AuthDto.AuthResponse> VerifyEmailAsync(string token)
    {
        User? user = await _userRepository.GetUserByVerificationTokenAsync(token);

        if (user == null || user.VerificationTokenExpiresAt < DateTime.UtcNow)
            throw new InvalidVerificationTokenException();

        user.IsVerified = true;
        user.VerificationToken = null;
        user.VerificationTokenExpiresAt = null;

        await _userRepository.UpdateUserAsync();
        _logger.LogInformation($"Email verified for user id: {user.Id}");

        string accessToken = await _tokenService.GenerateAccessToken(user);
        RefreshToken refreshToken = await _tokenService.GenerateRefreshToken();
        user.RefreshTokens.Add(refreshToken);
        await _userRepository.UpdateUserAsync();

        return new AuthDto.AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<AuthDto.AuthResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation($"Login attempt with email: {request.Email}");
        bool userExists = _userRepository.ExistsAsync(request.Email).Result;
        if (!userExists)
            throw new InvalidCredentialsException();

        User? user = await _userRepository.GetUserByEmailAsync(request.Email);

        if (!user!.IsVerified)
            throw new EmailNotVerifiedException();
        string accessToken = await _tokenService.GenerateAccessToken(user);

        RefreshToken refreshToken = await _tokenService.GenerateRefreshToken();
        string refreshTokenString = refreshToken.Token;
        user.RefreshTokens.Add(refreshToken);
        await _userRepository.UpdateUserAsync();
        _logger.LogInformation($"Login success with email: {request.Email}");
        AuthDto.AuthResponse authResponse = new AuthDto.AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenString
        };

        return await Task.FromResult(authResponse);
    }


    public async Task<AuthDto.AuthResponse> RefreshTokenAsync(string refreshTokenString)
    {
        User? user = await _userRepository.GetUserByRefreshTokenAsync(refreshTokenString);
        if (user == null)
            throw new CredentialUnavailableException(
                "USER IN DB IS NULL"); // todo create exception
        RefreshToken? refreshToken = await _refreshTokenRepository.GetRefreshTokenByStringAsync(refreshTokenString);
        refreshToken?.RevokedAt = DateTime.UtcNow;

        RefreshToken newRefreshToken = await _refreshTokenRepository.CreateAsync(new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        });
        
        user.RefreshTokens.Add(newRefreshToken);

        string accessToken = await _tokenService.GenerateAccessToken(user);
        //todo mapping for authresponse 
        AuthDto.AuthResponse authResponse = new AuthDto.AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken?.Token
        };
        return await Task.FromResult(authResponse);
    }

    public async Task<AuthDto.RevokeUserDtoResponse> RevokeRefreshTokenFromUserByEmailAsync(string email)
    {
        return new AuthDto.RevokeUserDtoResponse();
    }

    public async Task ResendVerificationEmailAsync(string email)
    {
        User? user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null || user.IsVerified) return; // silently ignore — don't leak info

        if (user.VerificationEmailLastSentAt.HasValue &&
            DateTime.UtcNow - user.VerificationEmailLastSentAt.Value < TimeSpan.FromSeconds(30))
            throw new VerificationEmailCooldownException();

        string verificationToken = Guid.NewGuid().ToString();
        user.VerificationToken = verificationToken;
        user.VerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24);
        user.VerificationEmailLastSentAt = DateTime.UtcNow;
        await _userRepository.UpdateUserAsync();

        await _emailService.SendVerificationEmailAsync(user.Email, verificationToken);
    }

    public async Task ForgotPasswordAsync(string email)
    {
        User? user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null) return; // silently ignore — don't leak user existence

        string resetToken = Guid.NewGuid().ToString();
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);
        await _userRepository.UpdateUserAsync();
        await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);
    }

    public async Task ResetPasswordAsync(string token, string newPassword)
    {
        User? user = await _userRepository.GetUserByPasswordResetTokenAsync(token);
        if (user == null || user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
            throw new InvalidPasswordResetTokenException();

        byte[] salt = CryptographyHelper.GenerateSalt(16);
        user.PasswordHash = CryptographyHelper.HashPassword(newPassword, salt);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;
        await _userRepository.UpdateUserAsync();
    }
}