using VotingPoll.Core.Entities;

namespace VotingPoll.Infrastructure.Repositories;

public interface IVoteRepository
{
    public Task<List<Vote>> GetAllAsync(int pollId);
    public Task<List<Vote>> GetAllForPollOptionAsync(int pollId, int pollOptionId);
    public Task<Vote?> GetAsync(int pollId);
    public Task<Vote?> CreateAsync(Vote vote);
    public Task<bool> ExistsAsync(int pollId);
    public Task<bool> UserAlreadyVotedAsync(int pollId, string userId);
}