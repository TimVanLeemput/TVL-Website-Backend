using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Infrastructure.Data;

namespace VotingPoll.Infrastructure.Repositories;

public class VoteRepository : IVoteRepository
{
    private readonly AppDbContext _context;

    public VoteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vote>> GetAllAsync(int pollId, int? page = null, int? pageSize = null)
    {
        IQueryable<Vote> query = _context.Votes.AsNoTracking().Where(x => x.PollId == pollId)
            .OrderBy(x => x.Id);

        if (page != null || pageSize != null)
        {
            if (page == null || page == 0) page = 1;
            if (pageSize == null || pageSize == 0) pageSize = 10;
            query = query.Skip((page.Value - 1) *
                               pageSize.Value).Take(pageSize.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Vote?> GetByIdAsync(int voteId)
    {
        return await _context.Votes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == voteId);
    }

    public async Task<int> GetVoteCountForPollAsync(int pollId)
    {
        IQueryable<Vote> query = _context.Votes.Where(x => x.PollId == pollId);

        return await query
            .CountAsync();
    }

    public async Task<Vote> CreateAsync(Vote vote)
    {
        _context.Votes.Add(vote);
        await _context.SaveChangesAsync();

        return vote;
    }

    public Task<bool> ExistsAsync(int voteId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UserAlreadyVotedAsync(int pollId, string userId)
    {
        return await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId);
    }
}