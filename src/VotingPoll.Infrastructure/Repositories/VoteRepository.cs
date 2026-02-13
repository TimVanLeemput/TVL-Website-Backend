using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Infrastructure.Data;

namespace VotingPoll.Infrastructure.Repositories;

public class VoteRepository : IVoteRepository
{
    AppDbContext _context;

    public VoteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vote>> GetAllAsync(int pollId)
    {
        List<Vote> votes = await _context.Votes.AsNoTracking().Where(x => x.PollId == pollId).ToListAsync();
        return votes;
    }

    public async Task<Vote?> GetAsync(int pollId)
    {
        return await _context.Votes.AsNoTracking().FirstOrDefaultAsync(x => x.PollId == pollId);
    }

    public Task<List<Vote>> GetAllForPollOptionAsync(int pollId, int pollOptionId)
    {
        throw new NotImplementedException();
    }

    public async Task<Vote?> CreateAsync(Vote? vote)
    {
        _context.Votes.Add(vote!);
        await _context.SaveChangesAsync();
        
        return await Task.FromResult(vote);
    }

    public Task<bool> ExistsAsync(int pollId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UserAlreadyVotedAsync(int pollId, string userId)
    {
        return await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId);
    }
}