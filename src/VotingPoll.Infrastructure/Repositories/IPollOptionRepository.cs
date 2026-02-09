using VotingPoll.Core.Entities;
namespace VotingPoll.Infrastructure.Repositories;

public interface IPollOptionRepository
{
    Task<List<PollOption>> GetAllAsync(int pollId);
    Task<PollOption> GetAsync(int id);
    Task<PollOption> SaveAsync(PollOption pollOption);
    Task<PollOption> UpdateAsync(int id, PollOption pollOption);
    Task<PollOption> DeleteAsync(int id);
}