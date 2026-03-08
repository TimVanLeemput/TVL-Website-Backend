using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Infrastructure.Data;

namespace VotingPoll.Infrastructure.Repositories;

// In Infrastructure project — defines HOW they work
public class PollRepository : IPollRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<PollRepository> _logger;

    public PollRepository(AppDbContext context, ILogger<PollRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Poll?> GetByIdAsync(int id)
    {
        return await _context.Polls
            .Include(x => x.AllPollOptions)!
            .ThenInclude(x => x.AllVotes)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Poll>> GetAllAsync(bool? isOpen = null, int? page = null, int? pageSize = null)
    {
        IQueryable<Poll> query = _context.Polls
            .Include(x => x.AllPollOptions)
            .ThenInclude(x => x.AllVotes)
            .AsSplitQuery()
            .OrderBy(x => x.Id);

        query = ApplyFilters(query, isOpen);

        if (page != null || pageSize != null)
        {
            if (page == null || page == 0) page = 1;
            if (pageSize == null || pageSize == 0) pageSize = 10;
            query = query.Skip((page.Value - 1) *
                               pageSize.Value).Take(pageSize.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<int> GetAllPollsCountAsync(bool? isOpen = null)
    {
        IQueryable<Poll> query = _context.Polls;

        query = ApplyFilters(query, isOpen);

        return await query
            .CountAsync();
    }

    private IQueryable<Poll> ApplyFilters(IQueryable<Poll> query,
        bool? isOpen)
    {
        if (isOpen == true)
            query = query.Where(x => x.ClosesAt == null || x.ClosesAt > DateTime.UtcNow);
        else if (isOpen == false)
            query = query.Where(x => x.ClosesAt != null && x.ClosesAt < DateTime.UtcNow);

        return query;
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

    public async Task<Poll?> GetByWeekNumberAsync(int weekNumber)
    {
        return await _context.Polls
            .Include(x => x.AllPollOptions)!
            .ThenInclude(x => x.AllVotes)
            .FirstOrDefaultAsync(x => x.WeekNumber == weekNumber);
    }
}