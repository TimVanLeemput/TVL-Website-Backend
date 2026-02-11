using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.Entities;
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
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Poll> CreateAsync(Poll poll)
    {
        _context.Polls.Add(poll);
        await _context.SaveChangesAsync();
        return poll;
    }
    
    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        Poll? pollToDelete = await _context.Polls.FindAsync(id);
        if (pollToDelete == null)
            throw new Exception("Poll not found");
        
        _context.Polls.Remove(pollToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Polls.AnyAsync(p => p.Id == id);
    }

}