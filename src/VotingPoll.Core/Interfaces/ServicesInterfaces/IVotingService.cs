using VotingPoll.Core.DTOs;

namespace VotingPoll.Core.Interfaces.ServicesInterfaces;

public interface IVotingService
{
    public Task<VoteConfirmationDto> Create(int pollId, CreateVoteDto createVoteDto);
}