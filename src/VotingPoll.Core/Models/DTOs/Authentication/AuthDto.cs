namespace VotingPoll.Core.Models.DTOs.Authentication;

public class AuthDto
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}