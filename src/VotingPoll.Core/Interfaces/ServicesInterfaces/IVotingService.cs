using VotingPoll.Core.DTOs;

namespace VotingPoll.Core.Interfaces.ServicesInterfaces;

public interface IVotingService
{
    public Task<VoteDto> GetById(int id);
    public Task<List<VoteDto>> GetAllVotesForPoll(int pollId);

    public Task<VoteConfirmationDto> Create(int pollId, CreateVoteDto createVoteDto);
}