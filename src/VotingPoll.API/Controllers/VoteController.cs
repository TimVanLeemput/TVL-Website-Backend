using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using VotingPoll.Core.DTOs;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Exceptions;
using VotingPoll.Core.Mappings;
using VotingPoll.Infrastructure.Repositories;
using VotingPoll.Infrastructure.Validation;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/polls/{pollId}/vote")]
public class VoteController : ControllerBase
{
    private readonly IVoteRepository _voteRepository;
    private readonly IPollRepository _pollRepository;
    private readonly IPollOptionRepository _pollOptionRepository;
    private readonly CreateVoteRequestValidator _createVoteRequestValidator;
    private readonly ILogger<VoteController> _logger;

    public VoteController(IVoteRepository voteRepository, IPollRepository pollRepository,
        IPollOptionRepository pollOptionRepository,
        CreateVoteRequestValidator createVoteRequestValidator,
        ILogger<VoteController> logger)
    {
        _pollRepository = pollRepository;
        _voteRepository = voteRepository;
        _pollOptionRepository = pollOptionRepository;
        _createVoteRequestValidator = createVoteRequestValidator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<VoteDto>>> GetAllVotesForPoll(int pollId)
    {
        // List<Vote> votes = await _context.Votes
        //     .Where(v => v.PollId == pollId)
        //     .ToListAsync();
        List<Vote?> votes = await _voteRepository.GetAllAsync(pollId);

        List<VoteDto> votesDto = votes.Select(vote => new VoteDto
        {
            Id = vote.Id,
            PollOptionId = vote.PollOptionId,
            UserId = vote.UserId,
            PollId = vote.PollId,
            VotedAt = vote.VotedAt
        }).ToList();

        return Ok(votesDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VoteDto>?> GetById(int id)
    {
        Vote? voteToGet = await _voteRepository.GetAsync(id);
        if (voteToGet == null) return NotFound();

        VoteDto voteDto = new VoteDto
        {
            Id = voteToGet.Id,
            PollOptionId = voteToGet.PollOptionId,
            UserId = voteToGet.UserId,
            VotedAt = voteToGet.VotedAt,
            PollId = voteToGet.PollId,
        };

        return Ok(voteDto);
    }


    [HttpPost]
    public async Task<ActionResult<VoteDto>> Create(int pollId, CreateVoteDto createVoteDto)
    {
        // Verify poll exists
        Poll? poll = await _pollRepository.GetByIdAsync(pollId);
        if (poll == null)
            throw new PollNotFoundException(pollId);
        // return NotFound("Poll not found");

        if (poll.ClosesAt < DateTime.UtcNow)
            throw new PollClosedException(pollId);

        bool userAlreadyVoted = await _voteRepository.UserAlreadyVotedAsync(pollId, createVoteDto.UserId);
        if (userAlreadyVoted)
            throw new AlreadyVotedException(createVoteDto.UserId);

        ValidationResult validationResult = await _createVoteRequestValidator.ValidateAsync(createVoteDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        PollOption? option = await _pollOptionRepository.GetAsync(createVoteDto.PollOptionId);
        if (option == null || option.PollId != pollId)
            return BadRequest("Poll option does not belong to this poll");

        Vote createdVote = createVoteDto.ToEntity(pollId);
        _logger.LogInformation($"Created vote for poll with id {pollId}");

        await _voteRepository.CreateAsync(createdVote);

        VoteDto voteDto = new VoteDto
        {
            Id = createdVote.Id,
            PollOptionId = createdVote.PollOptionId,
            UserId = createdVote.UserId,
            VotedAt = createdVote.VotedAt,
            PollId = createdVote.PollId,
        };

        return CreatedAtAction(nameof(GetById), new { pollId = createdVote.PollId, id = createdVote.Id },
            voteDto);
    }
}