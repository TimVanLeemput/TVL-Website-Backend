using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using VotingPoll.Core.Interfaces.Authentication;
using VotingPoll.Core.Models.DTOs.Authentication;

namespace VotingPoll.API.Controllers.Authentication;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthDto.AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        AuthDto.AuthResponse response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthDto.AuthResponse>> Login(LoginRequest request)
    {
        AuthDto.AuthResponse response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthDto.AuthResponse>> Refresh(AuthDto.RefreshTokenRequestDto refreshTokenRequestDto)
    {
        AuthDto.AuthResponse response = await _authService.RefreshTokenAsync(refreshTokenRequestDto.RefreshToken);
        return Ok(response);
    }
}