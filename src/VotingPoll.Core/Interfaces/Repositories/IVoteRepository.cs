using VotingPoll.Core.Entities;

namespace VotingPoll.Core.Interfaces.Repositories;

public interface IVoteRepository
{
    public Task<List<Vote>> GetAllAsync(int pollId, int? page = null, int? pageSize = null);
    public Task<int> GetAllForPollOptionAsync(int pollId);
    public Task<Vote?> GetAsync(int pollId);
    public Task<Vote?> CreateAsync(Vote vote);
    public Task<bool> ExistsAsync(int pollId);
    public Task<bool> UserAlreadyVotedAsync(int pollId, string userId);
}