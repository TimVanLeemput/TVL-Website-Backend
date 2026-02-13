using Microsoft.AspNetCore.Mvc;
using VotingPoll.Core.DTOs;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Infrastructure.Validation;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PollsController : ControllerBase
{
    IPollService _pollService;
    CreatePollDtoValidator _createPollDtoValidator;
    UpdatePollValidator _updatePollValidator;

    public PollsController(IPollService pollService,
        CreatePollDtoValidator createPollDtoValidator,
        UpdatePollValidator updatePollValidator)
    {
        _pollService = pollService;
        _createPollDtoValidator = createPollDtoValidator;
        _updatePollValidator = updatePollValidator;
    }

    #region GET

    [HttpGet]
    public async Task<ActionResult<List<PollDto>>> GetAll()
    {
        return await _pollService.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PollDto>> GetById(int id)
    {
        PollDto pollDto = await _pollService.GetById(id);

        return Ok(pollDto);
    }

    [HttpGet("{id}/creationDate")]
    public async Task<ActionResult<PollCreationDateDto>> GetPollCreationDateById(int id)
    {
        PollCreationDateDto pollCreationDateDto = await _pollService.GetPollCreationDateById(id);
        return Ok(pollCreationDateDto);
    }

    #endregion

    #region Create

    [HttpPost]
    public async Task<ActionResult<PollDto>> Create(CreatePollDto
        createPollDto)
    {
        ValidationResult validationResult = await _createPollDtoValidator.ValidateAsync(createPollDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        PollDto pollDto = await _pollService.Create(createPollDto);

        return CreatedAtAction(nameof(GetById), new { id = pollDto.PollId }, pollDto);
    }



    #endregion

    #region UpdatePoll

    [HttpPut("{id}")]
    public async Task<ActionResult<PollDto>> UpdatePoll(int id, UpdatePollDto? pollIn)
    {
        if (pollIn != null)
        {
            ValidationResult validationResult = await _updatePollValidator.ValidateAsync(pollIn);
            if (!validationResult.IsValid)
                return BadRequest(validationResult);
        }

        PollDto pollToUpdate = await _pollService.UpdatePoll(id, pollIn);
        return Ok(pollToUpdate);
    }

    #endregion

    #region Delete

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _pollService.DeletePoll(id);
        return NoContent();
    }

    #endregion
}