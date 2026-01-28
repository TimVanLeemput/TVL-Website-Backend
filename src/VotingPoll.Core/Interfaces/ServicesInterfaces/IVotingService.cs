using VotingPoll.Core.Models;
using VotingPoll.Core.Models.DTOs;

namespace VotingPoll.Core.Interfaces.ServicesInterfaces;

public interface IVotingService
{
    public Task<VoteDto> GetById(int id);
    public Task<PagedList<VoteDto>> GetAllVotesForPoll(int pollId, int? page = null, int? pageSize = null);

    public Task<VoteConfirmationDto> Create(int? userId, int pollId, CreateVoteDto createVoteDto);
}