using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using VotingPoll.Core.Exceptions;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Models.DTOs;
using VotingPoll.Infrastructure.Validation;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/polls/{pollId}/options")]
public class PollOptionsController : ControllerBase
{
    private readonly IPollService _pollService;
    private readonly IPollOptionService _pollOptionService;

    private readonly CreatePollOptionDtoValidator _createPollOptionDtoValidator;

    public PollOptionsController(IPollOptionService pollOptionService,
        IPollService pollService, CreatePollOptionDtoValidator createPollOptionDtoValidator)
    {
        _pollOptionService = pollOptionService;
        _pollService = pollService;
        _createPollOptionDtoValidator = createPollOptionDtoValidator;
    }

    #region GET

    [HttpGet]
    public async Task<ActionResult<List<PollOptionDto>>> GetAllOptionsForPoll(int pollId)
    {
        List<PollOptionDto> pollOptionsDto = await _pollOptionService.GetAllPollOptionsForPoll(pollId);

        return Ok(pollOptionsDto);
    }

    [HttpGet("{pollOptionId}")]
    public async Task<ActionResult<PollOptionDto>> GetPollOption(int pollId, int pollOptionId)
    {
        PollOptionDto pollOptionDto = await _pollOptionService.GetById(pollId, pollOptionId);
        return Ok(pollOptionDto);
    }

    #endregion

    #region POST

    [HttpPost]
    public async Task<ActionResult<PollOptionDto>> CreatePollOption(int pollId, CreatePollOptionDto createPollOptionDto)
    {
        ValidationResult validationResult = await _createPollOptionDtoValidator.ValidateAsync(createPollOptionDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        PollOptionDto pollOption = await _pollOptionService.CreatePollOption(pollId, createPollOptionDto);
        return CreatedAtAction(nameof(GetPollOption), new { pollId, pollOptionId = pollOption.Id }, pollOption);
    }

    #endregion

    #region Delete

    [HttpDelete("{pollOptionId}")]
    public async Task<ActionResult> DeletePollOption(int pollId, int pollOptionId)
    {
        string deletedPollOptionName = await _pollOptionService.DeletePollOption(pollId, pollOptionId);
        return Ok($"{deletedPollOptionName} successfully deleted");
    }

    #endregion
}