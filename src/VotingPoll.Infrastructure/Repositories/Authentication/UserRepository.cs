using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.Entities.Authentication;
using VotingPoll.Core.Interfaces.Repositories.Authentication;
using VotingPoll.Infrastructure.Data;

namespace VotingPoll.Infrastructure.Repositories.Authentication;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserById(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User> CreateUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> Exists(int id)
    {
        bool userIdExists = await _context.Users.AnyAsync(x => x.Id == id);
        if (userIdExists)
            return true;
        return false;
    }

    public async Task<bool> Exists(string email)
    {
        bool userIdExists = await _context.Users.AnyAsync(x => x.Email == email);
        if (userIdExists)
            return true;
        return false;    }

    public async Task UpdateUser()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(int id)
    {
        User userToDelete = await GetUserById(id);
        _context.Users.Remove(userToDelete);
        await _context.SaveChangesAsync();
    }
}