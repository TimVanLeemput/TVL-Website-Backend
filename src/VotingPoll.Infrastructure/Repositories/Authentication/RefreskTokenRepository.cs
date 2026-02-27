using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.Entities.Authentication;
using VotingPoll.Core.Interfaces.Repositories.Authentication;
using VotingPoll.Infrastructure.Data;

namespace VotingPoll.Infrastructure.Repositories.Authentication;

public class RefreskTokenRepository : IRefreshTokenRepository
{
    private AppDbContext _context;

    public RefreskTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetRefreshTokenByStringAsync(string refreshToken)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
    }

    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        await _context.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }
}