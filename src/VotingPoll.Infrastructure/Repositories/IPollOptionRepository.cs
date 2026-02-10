using VotingPoll.Core.Entities;

namespace VotingPoll.Infrastructure.Repositories;

public interface IPollOptionRepository
{
    Task<List<PollOption?>> GetAllAsync(int pollId);
    Task<PollOption?> GetAsync(int id);
    Task<bool> ExistsAsync(int pollId);
    Task DeleteAsync(int id);
}