using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.DTOs;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Exceptions;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Infrastructure.Data;

namespace VotingPoll.Infrastructure.Repositories;

public class PollOptionRepository : IPollOptionRepository
{
    AppDbContext _context;

    public PollOptionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PollOption?>> GetAllAsync()
    {
        return await _context.PollOptions.AsNoTracking().ToListAsync();
    }

    public async Task<List<PollOption?>> GetAllPollOptionsForPollAsync(int pollId)
    {
        return await _context.PollOptions.AsNoTracking().Where(x => x.PollId == pollId).ToListAsync();
    }

    public async Task<PollOption?> GetAsync(int pollId, int pollOptionId)
    {
        PollOption? pollOption = await _context.PollOptions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == pollOptionId && x.PollId == pollId);
        return pollOption;
    }

    public async Task<PollOption> CreateAsync(PollOption pollOption)
    {
        await _context.PollOptions.AddAsync(pollOption);
        await _context.SaveChangesAsync();
        return await Task.FromResult(pollOption);
    }

    public async Task<bool> ExistsAsync(int pollId)
    {
        return await _context.PollOptions.AnyAsync(p => p.Id == pollId);
    }

    public async Task DeleteAsync(int pollId, int pollOptionId)
    {
        Poll? poll = await _context.Polls.FindAsync(pollId);
        if (poll == null)
            throw new PollNotFoundException(pollId);

        PollOption? pollOption = await _context.PollOptions.FindAsync(pollOptionId);
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