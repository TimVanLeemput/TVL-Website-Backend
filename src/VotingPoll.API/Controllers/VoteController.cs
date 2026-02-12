using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using VotingPoll.Core.DTOs;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Infrastructure.Repositories;
using VotingPoll.Infrastructure.Validation;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/polls/{pollId}/vote")]
public class VoteController : ControllerBase
{
    private readonly IVotingService _votingService;

    private readonly IVoteRepository _voteRepository;
    private readonly CreateVoteRequestValidator _createVoteRequestValidator;

    public VoteController(IVotingService votingService, IVoteRepository voteRepository,
        CreateVoteRequestValidator createVoteRequestValidator)
    {
        _votingService = votingService;
        _voteRepository = voteRepository;
        _createVoteRequestValidator = createVoteRequestValidator;
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

        VoteDto voteDto = await _votingService.GetById(id);

        return Ok(voteDto);
    }


    [HttpPost]
    public async Task<ActionResult<VoteConfirmationDto>> Create(int pollId, CreateVoteDto createVoteDto)
    {
        ValidationResult validationResult = await _createVoteRequestValidator.ValidateAsync(createVoteDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        return await _votingService.Create(pollId, createVoteDto);
    }
}