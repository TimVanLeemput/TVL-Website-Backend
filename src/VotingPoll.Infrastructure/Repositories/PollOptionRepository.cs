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

    public async Task<List<PollOption>> GetAllAsync(int pollId)
    {
        return await _context.PollOptions.AsNoTracking().Where(x => x.PollId == pollId).ToListAsync();
    }

    public async Task<PollOption> GetAsync(int id)
    {
        PollOption pollOption = await _context.PollOptions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return pollOption;
    }

    public Task<PollOption> SaveAsync(PollOption pollOption)
    {
        throw new NotImplementedException();
    }

    public Task<PollOption> UpdateAsync(int id, PollOption pollOption)
    {
        throw new NotImplementedException();
    }

    public Task<PollOption> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Exists(int pollId)
    {
        return await _context.Polls.AnyAsync(p => p.Id == pollId);
    }
}