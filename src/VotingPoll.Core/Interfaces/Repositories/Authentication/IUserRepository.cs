using VotingPoll.Core.Entities.Authentication;

namespace VotingPoll.Core.Interfaces.Repositories.Authentication;

public interface IUserRepository
{
    public Task<User> GetUserById(int id);
    public Task<User?> GetUserByEmail(string email);
    public Task<User> CreateUser(User user);
    public Task<bool> Exists(int id);
    public Task<bool> Exists(string email);
    public Task UpdateUser();
    public Task DeleteUser(int id);
}