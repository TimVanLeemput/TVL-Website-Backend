using Microsoft.Extensions.Logging;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Exceptions;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Mappings;
using VotingPoll.Core.Models;
using VotingPoll.Core.Models.DTOs;

namespace VotingPoll.Core.Services;

public class VotingService : IVotingService
{
    private readonly ILogger<VotingService> _logger;

    private readonly IVoteRepository _voteRepository;
    private readonly IPollRepository _pollRepository;
    private readonly IPollOptionRepository _pollOptionRepository;

    public VotingService(ILogger<VotingService> logger, IVoteRepository voteRepository, IPollRepository pollRepository,
        IPollOptionRepository pollOptionRepository)
    {
        _logger = logger;
        _voteRepository = voteRepository;
        _pollRepository = pollRepository;
        _pollOptionRepository = pollOptionRepository;
    }

    public async Task<VoteDto> GetById(int id)
    {
        Vote? vote = await _voteRepository.GetByIdAsync(id);
        if (vote == null)
            throw new VoteNotFoundException(id);

        VoteDto voteDto = vote.ToDto();

        return voteDto;
    }

    public async Task<PagedList<VoteDto>> GetAllVotesForPoll(int pollId, int? page = null, int? pageSize = null)
    {
        int totalCount = await _voteRepository.GetVoteCountForPollAsync(pollId);
        List<Vote> votes = await _voteRepository.GetAllAsync(pollId, page, pageSize);

        List<VoteDto> votesDto = votes.ToListOfVotesDto();
        PagedList<VoteDto> pagedListOfVotesDto = new PagedList<VoteDto>(votesDto, totalCount, page, pageSize);
        return pagedListOfVotesDto;
    }

    public async Task<VoteConfirmationDto> Create(int pollId, CreateVoteDto createVoteDto)
    {
        Poll? poll = await _pollRepository.GetByIdAsync(pollId);
        if (poll == null)
            throw new PollNotFoundException(pollId);

        if (poll.ClosesAt < DateTime.UtcNow)
            throw new PollClosedException(pollId);

        bool userAlreadyVoted = await _voteRepository.UserAlreadyVotedAsync(pollId, createVoteDto.UserId);
        if (userAlreadyVoted)
            throw new AlreadyVotedException(createVoteDto.UserId);

        PollOption option = await _pollOptionRepository.GetAsync(pollId, createVoteDto.PollOptionId);
        if (option == null)
            throw new InvalidPollOptionException();

        Vote vote = createVoteDto.ToEntity(pollId);
        _logger.LogInformation($"Created vote for poll with id {pollId}");

        await _voteRepository.CreateAsync(vote);

        VoteConfirmationDto voteConfirmationDto = poll.ToVoteConfirmationDto(option);

        return voteConfirmationDto;
    }
}