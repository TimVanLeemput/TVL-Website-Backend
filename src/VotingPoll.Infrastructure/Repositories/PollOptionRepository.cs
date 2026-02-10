using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.Entities;
using VotingPoll.Infrastructure.Data;

namespace VotingPoll.Infrastructure.Repositories;

public class PollOptionRepository : IPollOptionRepository
{
    AppDbContext _context;

    public PollOptionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PollOption?>> GetAllAsync(int pollId)
    {
        return await _context.PollOptions.AsNoTracking().Where(x => x.PollId == pollId).ToListAsync();
    }

    public async Task<PollOption?> GetAsync(int id)
    {
        PollOption pollOption = await _context.PollOptions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return pollOption;
    }

    public async Task<bool> ExistsAsync(int pollId)
    {
        return await _context.Polls.AnyAsync(p => p.Id == pollId);
    }

    public async Task DeleteAsync(int id)
    {
        PollOption pollOption = await _context.PollOptions.FindAsync(id);
        if (pollOption == null)
            throw new Exception("PollOption not found");
        _context.PollOptions.Remove(pollOption);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Exists(int pollId)
    {
        return await _context.Polls.AnyAsync(p => p.Id == pollId);
    }
}