using VotingPoll.Core.Entities;

namespace VotingPoll.Infrastructure.Repositories;

// In Core project — defines WHAT operations exist
public interface IPollRepository
{
    Task<Poll?> GetByIdAsync(int id);
    Task<List<Poll>> GetAllAsync();
    Task<Poll> CreateAsync(Poll poll);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task SaveChanges();
}