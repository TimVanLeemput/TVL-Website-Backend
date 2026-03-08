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

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string? refreshTokenString)
    {
        RefreshToken? refreshToken = await _context.RefreshTokens.Include(refreshToken => refreshToken.User)
            .FirstOrDefaultAsync(x => x.Token == refreshTokenString);
        User user = refreshToken.User;
        return user;
    }
    //todo remove
    // public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    // {
    //     // return await _context.Users.FirstOrDefaultAsync(x => x.RefreshTokens.Where(x => x.Token) == refreshToken);
    // }

    public async Task<User> CreateUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        bool userIdExists = await _context.Users.AnyAsync(x => x.Id == id);
        if (userIdExists)
            return true;
        return false;
    }

    public async Task<bool> ExistsAsync(string email)
    {
        bool userIdExists = await _context.Users.AnyAsync(x => x.Email == email);
        if (userIdExists)
            return true;
        return false;
    }

    public async Task UpdateUserAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        User userToDelete = await GetUserByIdAsync(id);
        _context.Users.Remove(userToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByVerificationTokenAsync(string token)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.VerificationToken == token);
    }
}