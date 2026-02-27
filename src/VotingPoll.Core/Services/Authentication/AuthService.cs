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

namespace VotingPoll.Core.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthService> _logger;

    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(ITokenService tokenService, IUserRepository userRepository, ILogger<AuthService> logger,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthDto.AuthResponse> RegisterAsync(RegisterRequest request)
    {
        //todo
        // 3. Hash password with cryptography
        // 4. Create and save User
        // 5. Generate tokens
        // 6. Save RefreshToken
        // 7. Return AuthResponse

        //Check duplicate email
        if (_userRepository.ExistsAsync(request.Email).Result)
        {
            throw new EmailAlreadyExistsException($"{request.Email}");
        }

        // var argon2 = new Argon2i();
        string passWord = request.Password;
        byte[] salt = CryptographyHelper.GenerateSalt(16);
        byte[] hashedPassword = CryptographyHelper.HashPassword(passWord, salt);
        _logger.LogInformation($"Generated hash: {Convert.ToBase64String(hashedPassword)}");

        User createdUser = new User
        {
            Id = 0,
            Email = request.Email,
            PasswordHash = hashedPassword,
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            RefreshTokens = new List<RefreshToken>()
        };

        await _userRepository.CreateUserAsync(createdUser);
        _logger.LogInformation($"Created user with id: {createdUser.Id}");

        string accessToken = await _tokenService.GenerateAccessToken(createdUser);
        RefreshToken refreshToken = await _tokenService.GenerateRefreshToken();
        string refreshTokenString = refreshToken.Token;

        AuthDto.AuthResponse authResponse = new AuthDto.AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenString
        };

        return await Task.FromResult(authResponse);
    }

    public async Task<AuthDto.AuthResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation($"Login attempt with email: {request.Email}");
        bool userExists = _userRepository.ExistsAsync(request.Email).Result;
        if (!userExists)
        {
            throw new InvalidCredentialsException();
        }

        User? user = await _userRepository.GetUserByEmailAsync(request.Email);
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
}