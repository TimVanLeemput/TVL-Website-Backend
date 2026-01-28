namespace VotingPoll.Core.Entities.Authentication;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = [];
    public Role Role { get; set; } = Role.User; // "User" or "Admin"
    public DateTime CreatedAt { get; set; }

    public bool IsVerified { get; set; } = false;
    public string? VerificationToken { get; set; }
    public DateTime? VerificationTokenExpiresAt { get; set; }
    public DateTime? VerificationEmailLastSentAt { get; set; }

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiresAt { get; set; }

    // Refresh tokens
    public List<RefreshToken?> RefreshTokens { get; set; } = new List<RefreshToken?>();
}

public enum Role
{
    Admin = 0,
    User = 1
}
