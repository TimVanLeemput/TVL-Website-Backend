using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Identity.Data;
using VotingPoll.Core.Entities.Authentication;
using VotingPoll.Core.Interfaces.Authentication;
using VotingPoll.Core.Interfaces.Repositories.Authentication;
using VotingPoll.Core.Interfaces.ServicesInterfaces.Authentication;
using VotingPoll.Core.Models.DTOs.Authentication;

namespace VotingPoll.Core.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;


    public AuthService(ITokenService tokenService, IUserRepository userRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public Task<AuthDto.AuthResponse> RegisterAsync(RegisterRequest request)
    {
        //Check duplicate email
        if (_userRepository.Exists(request.Email).Result)
        {
            throw new Exception("Email already exists"); //todo add to global exception handler
        }

        // var argon2 = new Argon2i();
        string passWord = request.Password;
        byte[] passwordByteArray = new byte[passWord.Length];
        for (int i = 0; i < passWord.Length; i++)
        {
            passwordByteArray[i] = Convert.ToByte(passWord[i]);
            var hashAlgo = new Argon2i(passwordByteArray);
            hashAlgo.Salt = new byte[16];
            var hash = hashAlgo.GetBytes(128);

            User createdUser = new User
            {
                Id = 0,
                Email = request.Email,
                PasswordHash = hash.ToString(),
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                RefreshTokens = new List<RefreshToken>()
            };

            _userRepository.CreateUser(createdUser);
        }


        return Task.FromResult(new AuthDto.AuthResponse());
    }

    public Task<AuthDto.AuthResponse> LoginAsync(LoginRequest request)
    {
        return Task.FromResult(new AuthDto.AuthResponse());
    }

    public Task<AuthDto.AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        return Task.FromResult(new AuthDto.AuthResponse());
    }
}