using VotingPoll.Core.Entities;

namespace VotingPoll.Core.Interfaces.Repositories;

// In Core project — defines WHAT operations exist
public interface IPollRepository
{
    Task<Poll?> GetByIdAsync(int id);
    Task<List<Poll>> GetAllAsync(bool? isOpen = null, int? page = null, int? pageSize = null);
    Task<int> GetAllPollsCountAsync(bool ? isOpen = null);
    Task<Poll> CreateAsync(Poll poll);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task UpdatePoll();
    Task<Poll?> GetByWeekNumberAsync(int weekNumber);
}