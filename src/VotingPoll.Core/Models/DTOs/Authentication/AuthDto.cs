namespace VotingPoll.Core.Models.DTOs.Authentication;

public class AuthDto
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
    }
    
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
        
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
    
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

}