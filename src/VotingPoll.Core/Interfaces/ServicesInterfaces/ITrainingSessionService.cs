using VotingPoll.Core.Models;
using VotingPoll.Core.Models.DTOs;

namespace VotingPoll.Core.Interfaces.ServicesInterfaces;

public interface ITrainingSessionService
{
    Task<TrainingSessionCreateResult> Create(CreateTrainingSessionDto createDto);
    Task<TrainingSessionDetailDto> GetById(int id);

    Task<PagedList<TrainingSessionSummaryDto>> GetAll(string? procedureId, DateTime? fromUtc, DateTime? toUtc,
        int? page = null, int? pageSize = null);
}
