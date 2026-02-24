using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using VotingPoll.Core.Interfaces.Authentication;

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
    public async Task<ActionResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        AuthResponse response = await _authService.RegisterAsync(request);
        return  Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        AuthResponse response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request)
    {
        AuthResponse response = await _authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(response);
    }


}