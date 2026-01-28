using Microsoft.AspNetCore.Identity.Data;
using VotingPoll.Core.Models.DTOs.Authentication;

namespace VotingPoll.Core.Interfaces.Authentication;

public interface IAuthService
{
    public Task<AuthDto.AuthResponse> RegisterAsync(RegisterRequest request);

    public Task<AuthDto.AuthResponse> LoginAsync(LoginRequest request);

    public Task<AuthDto.AuthResponse> RefreshTokenAsync(string refreshTokenString);
    public Task<AuthDto.RevokeUserDtoResponse> RevokeRefreshTokenFromUserByEmailAsync(string refreshTokenString);
    public Task<AuthDto.AuthResponse> VerifyEmailAsync(string token);
    public Task ResendVerificationEmailAsync(string email);
    public Task ForgotPasswordAsync(string email);
    public Task ResetPasswordAsync(string token, string newPassword);
}

