using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Infrastructure.Data;

namespace VotingPoll.Infrastructure.Repositories;

// In Infrastructure project — defines HOW they work
public class PollRepository : IPollRepository
{
    private readonly AppDbContext _context;

    public PollRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Poll?> GetByIdAsync(int id)
    {
        return await _context.Polls
            .Include(x => x.AllPollOptions)!
            .ThenInclude(x => x.AllVotes)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Poll>> GetAllAsync()
    {
        return await _context.Polls
            .Include(x => x.AllPollOptions)
            .ThenInclude(x => x.AllVotes).ToListAsync();
    }

    public async Task<Poll> CreateAsync(Poll poll)
    {
        _context.Polls.Add(poll);
        await _context.SaveChangesAsync();
        return poll;
    }

    public async Task UpdatePoll()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Poll? pollToDelete = await _context.Polls.FindAsync(id);

        _context.Polls.Remove(pollToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Polls.AnyAsync(p => p.Id == id);
    }
}