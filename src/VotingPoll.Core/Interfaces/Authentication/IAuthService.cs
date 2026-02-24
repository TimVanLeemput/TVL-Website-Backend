using Microsoft.AspNetCore.Identity.Data;

namespace VotingPoll.Core.Interfaces.Authentication;

public interface IAuthService
{
    public Task<AuthResponse> RegisterAsync(RegisterRequest request);

    public Task<AuthResponse> LoginAsync(LoginRequest request);

    public Task<AuthResponse> RefreshTokenAsync(string refreshToken);
}

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}