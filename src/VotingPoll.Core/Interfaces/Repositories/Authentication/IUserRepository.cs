using VotingPoll.Core.Entities.Authentication;

namespace VotingPoll.Core.Interfaces.Repositories.Authentication;

public interface IUserRepository
{
    public Task<User> GetUserByIdAsync(int id);
    public Task<User?> GetUserByEmailAsync(string email);
    public Task<User?> GetUserByRefreshTokenAsync(string? refreshTokenString);
    public Task<User> CreateUserAsync(User user);
    public Task<bool> ExistsAsync(int id);
    public Task<bool> ExistsAsync(string email);
    public Task UpdateUserAsync();
    public Task DeleteUserAsync(int id);
    public Task<User?> GetUserByVerificationTokenAsync(string token);
    public Task<User?> GetUserByPasswordResetTokenAsync(string token);
}