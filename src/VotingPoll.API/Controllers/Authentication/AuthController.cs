using FluentValidation.Results;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using VotingPoll.Core.Entities.Authentication;
using VotingPoll.Core.Exceptions.AuthExceptions;
using VotingPoll.Core.Interfaces.Authentication;
using VotingPoll.Core.Models.DTOs.Authentication;
using VotingPoll.Core.Validation.AuthValidators;

namespace VotingPoll.API.Controllers.Authentication;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly RegisterRequestPasswordValidator _registerRequestPasswordValidator;

    public AuthController(IAuthService authService, ILogger<AuthController> logger,
        RegisterRequestPasswordValidator registerRequestPasswordValidator)
    {
        _authService = authService;
        _logger = logger;
        _registerRequestPasswordValidator = registerRequestPasswordValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthDto.AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        AuthDto.RegisterRequest registerRequestDto = new AuthDto.RegisterRequest
        {
            Email = request.Email,
            Password = request.Password,
        };

        ValidationResult validationResult = await _registerRequestPasswordValidator.ValidateAsync(registerRequestDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(error => error.ErrorMessage));

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
    
    //todo create revoke dto for User (RevokeUserDto?)
    [HttpPost("revoke")]
    public async Task<ActionResult> Revoke(AuthDto.RevokeUserDto revokeUserDto)
    {
        AuthDto.RevokeUserDtoResponse revokeUserDtoResponse = await _authService.RevokeRefreshTokenFromUserByEmailAsync(revokeUserDto.Email);
        return Ok();
    }
}