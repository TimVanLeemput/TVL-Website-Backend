using VotingPoll.Core.Entities.Training;

namespace VotingPoll.Core.Interfaces.Repositories;

public interface ITrainingSessionRepository
{
    Task<TrainingSession> CreateAsync(TrainingSession session);
    Task<TrainingSession?> GetByIdAsync(int id);
    Task<TrainingSession?> GetBySessionIdAsync(Guid sessionId);

    Task<List<TrainingSession>> GetAllAsync(string? procedureId, DateTime? fromUtc, DateTime? toUtc,
        int? page = null, int? pageSize = null);

    Task<int> GetCountAsync(string? procedureId, DateTime? fromUtc, DateTime? toUtc);
}
