namespace VotingPoll.Core.Entities.Authentication;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";  // "User" or "Admin"
    public DateTime CreatedAt { get; set; }

    // Refresh tokens
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
