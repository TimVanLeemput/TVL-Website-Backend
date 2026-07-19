using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingPoll.API.Filters;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Models;
using VotingPoll.Core.Models.DTOs;
using VotingPoll.Infrastructure.Validation;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/training-sessions")]
public class TrainingSessionsController : ControllerBase
{
    private readonly ITrainingSessionService _trainingSessionService;

    private readonly CreateTrainingSessionValidator _createTrainingSessionValidator;

    public TrainingSessionsController(ITrainingSessionService trainingSessionService,
        CreateTrainingSessionValidator createTrainingSessionValidator)
    {
        _trainingSessionService = trainingSessionService;
        _createTrainingSessionValidator = createTrainingSessionValidator;
    }

    #region GET

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedList<TrainingSessionSummaryDto>>> GetAll(string? procedureId = null,
        DateTime? fromUtc = null, DateTime? toUtc = null, int? page = null, int? pageSize = null)
    {
        PagedList<TrainingSessionSummaryDto> sessions =
            await _trainingSessionService.GetAll(procedureId, fromUtc, toUtc, page, pageSize);

        return Ok(sessions);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<TrainingSessionDetailDto>> GetById(int id)
    {
        TrainingSessionDetailDto sessionDto = await _trainingSessionService.GetById(id);

        return Ok(sessionDto);
    }

    #endregion

    #region POST

    // Device-key auth, not JWT: the headset has no user login. Idempotent on sessionId —
    // a re-upload from the offline queue returns 200 with the already-stored session.
    // Explicit size limit because the global Kestrel cap (10 KB) is tight for long procedures.
    [RequireDeviceKey]
    [RequestSizeLimit(64 * 1024)]
    [HttpPost]
    public async Task<ActionResult<TrainingSessionSummaryDto>> Create(
        CreateTrainingSessionDto createTrainingSessionDto)
    {
        ValidationResult validationResult =
            await _createTrainingSessionValidator.ValidateAsync(createTrainingSessionDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        TrainingSessionCreateResult result = await _trainingSessionService.Create(createTrainingSessionDto);

        if (!result.Created)
            return Ok(result.Session);

        return CreatedAtAction(nameof(GetById), new { id = result.Session.Id }, result.Session);
    }

    #endregion
}
