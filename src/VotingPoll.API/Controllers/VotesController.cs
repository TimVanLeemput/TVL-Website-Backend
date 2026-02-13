using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using VotingPoll.Core.DTOs;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Infrastructure.Validation;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/polls/{pollId}/vote")]
public class VotesController : ControllerBase
{
    private readonly IVotingService _votingService;

    private readonly CreateVoteRequestValidator _createVoteRequestValidator;

    public VotesController(IVotingService votingService,
        CreateVoteRequestValidator createVoteRequestValidator)
    {
        _votingService = votingService;
        _createVoteRequestValidator = createVoteRequestValidator;
    }

    #region GET

    [HttpGet]
    public async Task<ActionResult<List<VoteDto>>> GetAllVotesForPoll(int pollId)
    {
        List<VoteDto> votesDto = await _votingService.GetAllVotesForPoll(pollId);

        return Ok(votesDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VoteDto>?> GetById(int id)
    {
        VoteDto voteDto = await _votingService.GetById(id);

        return Ok(voteDto);
    }

    #endregion

    #region POST

    [HttpPost]
    public async Task<ActionResult<VoteConfirmationDto>> Create(int pollId, CreateVoteDto createVoteDto)
    {
        ValidationResult validationResult = await _createVoteRequestValidator.ValidateAsync(createVoteDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        return await _votingService.Create(pollId, createVoteDto);
    }

    #endregion
}