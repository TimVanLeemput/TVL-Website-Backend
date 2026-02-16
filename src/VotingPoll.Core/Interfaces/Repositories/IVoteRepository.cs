using VotingPoll.Core.Entities;

namespace VotingPoll.Core.Interfaces.Repositories;

public interface IVoteRepository
{
    public Task<List<Vote>> GetAllAsync(int pollId, int? page = null, int? pageSize = null);
    public Task<int> GetVoteCountForPollAsync(int pollId);
    public Task<Vote?> GetByIdAsync(int voteId);
    public Task<Vote> CreateAsync(Vote vote);
    public Task<bool> ExistsAsync(int voteId);
    public Task<bool> UserAlreadyVotedAsync(int pollId, string userId);
}