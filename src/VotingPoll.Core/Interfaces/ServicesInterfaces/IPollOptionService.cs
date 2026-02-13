using VotingPoll.Core.DTOs;

namespace VotingPoll.Core.Interfaces.ServicesInterfaces;

public interface IPollOptionService
{
    public Task<PollOptionDto> GetById(int pollId, int pollOptionId);
    public Task<List<PollOptionDto>> GetAllPollOptionsForPoll(int pollId);
    public Task<string> DeletePollOption(int pollId, int pollOptionId);
}