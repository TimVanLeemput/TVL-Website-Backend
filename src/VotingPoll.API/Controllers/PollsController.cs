using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Models;
using VotingPoll.Core.Models.DTOs;
using VotingPoll.Infrastructure.Validation;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PollsController : ControllerBase
{
    private readonly ILogger<PollsController> _logger;

    private readonly IPollService _pollService;

    private readonly CreatePollDtoValidator _createPollDtoValidator;
    private readonly UpdatePollValidator _updatePollValidator;

    public PollsController(IPollService pollService,
        CreatePollDtoValidator createPollDtoValidator,
        UpdatePollValidator updatePollValidator, ILogger<PollsController> logger)
    {
        _pollService = pollService;
        _createPollDtoValidator = createPollDtoValidator;
        _updatePollValidator = updatePollValidator;
        _logger = logger;
    }

    #region GET

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedList<PollDto>>> GetAll([FromQuery] bool? isOpen = null, int? page = null,
        int? pageSize = null)
    {
        return await _pollService.GetAll(isOpen, page, pageSize);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<PollDto>> GetById(int id)
    {
        PollDto pollDto = await _pollService.GetById(id);

        return Ok(pollDto);
    }

    [Authorize]
    [HttpGet("{id}/creationDate")]
    public async Task<ActionResult<PollCreationDateDto>> GetPollCreationDateById(int id)
    {
        PollCreationDateDto pollCreationDateDto = await _pollService.GetPollCreationDateById(id);
        return Ok(pollCreationDateDto);
    }

    [Authorize]
    [HttpGet("{id}/results")]
    public async Task<ActionResult<PollResultsDto>> GetPollResultsById(int id)
    {
        PollResultsDto pollResultsDto = await _pollService.GetPollResultsById(id);
        return Ok(pollResultsDto);
    }

    #endregion

    #region Create

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _pollService.DeletePoll(id);
        return NoContent();
    }

    #endregion
}