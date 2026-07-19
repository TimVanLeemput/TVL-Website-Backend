using Microsoft.Extensions.Logging;
using VotingPoll.Core.Entities.Training;
using VotingPoll.Core.Exceptions;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Mappings;
using VotingPoll.Core.Models;
using VotingPoll.Core.Models.DTOs;

namespace VotingPoll.Core.Services;

public class TrainingSessionService : ITrainingSessionService
{
    private readonly ILogger<TrainingSessionService> _logger;
    private readonly ITrainingSessionRepository _trainingSessionRepository;

    public TrainingSessionService(ILogger<TrainingSessionService> logger,
        ITrainingSessionRepository trainingSessionRepository)
    {
        _logger = logger;
        _trainingSessionRepository = trainingSessionRepository;
    }

    public async Task<TrainingSessionCreateResult> Create(CreateTrainingSessionDto createDto)
    {
        Guid sessionId = Guid.Parse(createDto.SessionId);

        // Idempotency: the headset's offline queue may re-upload after a crash or lost
        // response — the same sessionId must never produce a second row.
        TrainingSession? existing = await _trainingSessionRepository.GetBySessionIdAsync(sessionId);
        if (existing != null)
        {
            _logger.LogInformation(
                $"Training session {sessionId} already stored (id {existing.Id}) — idempotent replay.");
            return new TrainingSessionCreateResult { Session = existing.ToSummaryDto(), Created = false };
        }

        TrainingSession session = createDto.ToEntity(DateTime.UtcNow);
        await _trainingSessionRepository.CreateAsync(session);
        _logger.LogInformation(
            $"Stored training session {sessionId} for procedure '{session.ProcedureId}' (score {session.Score}, passed {session.Passed}).");

        return new TrainingSessionCreateResult { Session = session.ToSummaryDto(), Created = true };
    }

    public async Task<TrainingSessionDetailDto> GetById(int id)
    {
        TrainingSession? session = await _trainingSessionRepository.GetByIdAsync(id);
        if (session == null)
            throw new TrainingSessionNotFoundException(id);

        return session.ToDetailDto();
    }

    public async Task<PagedList<TrainingSessionSummaryDto>> GetAll(string? procedureId, DateTime? fromUtc,
        DateTime? toUtc, int? page = null, int? pageSize = null)
    {
        int totalCount = await _trainingSessionRepository.GetCountAsync(procedureId, fromUtc, toUtc);
        List<TrainingSession> sessions =
            await _trainingSessionRepository.GetAllAsync(procedureId, fromUtc, toUtc, page, pageSize);

        List<TrainingSessionSummaryDto> summaries = sessions.ToListOfSummaryDto();
        return new PagedList<TrainingSessionSummaryDto>(summaries, totalCount, page, pageSize);
    }
}
