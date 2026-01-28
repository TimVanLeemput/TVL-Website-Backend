namespace VotingPoll.Core.Interfaces.Repositories;

public interface IPollOptionRepository
{
    Task<List<PollOption?>> GetAllAsync();
    Task<List<PollOption?>> GetAllPollOptionsForPollAsync(int pollId);
    Task<PollOption?> GetAsync(int pollId, int pollOptionId);
    Task<PollOption> CreateAsync(PollOption pollOption);
    Task<bool> ExistsAsync(int pollId);
    Task DeleteAsync(int pollId, int pollOptionId);
}