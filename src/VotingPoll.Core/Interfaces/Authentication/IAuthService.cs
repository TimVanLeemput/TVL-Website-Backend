using Microsoft.AspNetCore.Identity.Data;
using VotingPoll.Core.Models.DTOs.Authentication;

namespace VotingPoll.Core.Interfaces.Authentication;

public interface IAuthService
{
    public Task<AuthDto.AuthResponse> RegisterAsync(RegisterRequest request);

    public Task<AuthDto.AuthResponse> LoginAsync(LoginRequest request);

    public Task<AuthDto.AuthResponse> RefreshTokenAsync(string refreshToken);
}

