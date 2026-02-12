using Microsoft.Extensions.Logging;
using VotingPoll.Core.DTOs;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Exceptions;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Mappings;
using VotingPoll.Infrastructure.Repositories;

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
        Vote? voteToGet = await _voteRepository.GetAsync(id);
        if (voteToGet == null)
            throw new VoteNotFoundException(id);

        VoteDto voteDto = new VoteDto
        {
            Id = voteToGet.Id,
            PollOptionId = voteToGet.PollOptionId,
            UserId = voteToGet.UserId,
            VotedAt = voteToGet.VotedAt,
            PollId = voteToGet.PollId,
        };

        return voteDto;
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

        PollOption? option = await _pollOptionRepository.GetAsync(createVoteDto.PollOptionId);
        if (option == null)
            throw new InvalidPollOptionException();
        if (option.PollId != pollId)
            throw new InvalidPollOptionException(option.PollId);

        Vote vote = createVoteDto.ToEntity(pollId);
        _logger.LogInformation($"Created vote for poll with id {pollId}");

        await _voteRepository.CreateAsync(vote);

        VoteConfirmationDto voteConfirmationDto = poll.ToVoteConfirmationDto(option);

        return voteConfirmationDto;
    }
}