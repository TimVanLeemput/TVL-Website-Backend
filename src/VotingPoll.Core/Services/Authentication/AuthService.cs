using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using VotingPoll.Core.Entities.Authentication;
using VotingPoll.Core.Interfaces.Authentication;
using VotingPoll.Core.Interfaces.ServicesInterfaces.Authentication;

namespace VotingPoll.Core.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;

    public AuthService(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }
}