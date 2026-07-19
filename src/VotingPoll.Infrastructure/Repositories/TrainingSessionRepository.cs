using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.Entities.Training;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Infrastructure.Data;

namespace VotingPoll.Infrastructure.Repositories;

public class TrainingSessionRepository : ITrainingSessionRepository
{
    private readonly AppDbContext _context;

    public TrainingSessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TrainingSession> CreateAsync(TrainingSession session)
    {
        _context.TrainingSessions.Add(session);
        await _context.SaveChangesAsync();

        return session;
    }

    public async Task<TrainingSession?> GetByIdAsync(int id)
    {
        return await _context.TrainingSessions.AsNoTracking()
            .Include(ts => ts.Steps)
            .Include(ts => ts.Errors)
            .FirstOrDefaultAsync(ts => ts.Id == id);
    }

    public async Task<TrainingSession?> GetBySessionIdAsync(Guid sessionId)
    {
        return await _context.TrainingSessions.AsNoTracking()
            .Include(ts => ts.Errors)
            .FirstOrDefaultAsync(ts => ts.SessionId == sessionId);
    }

    public async Task<List<TrainingSession>> GetAllAsync(string? procedureId, DateTime? fromUtc, DateTime? toUtc,
        int? page = null, int? pageSize = null)
    {
        IQueryable<TrainingSession> query = Filter(procedureId, fromUtc, toUtc)
            .Include(ts => ts.Errors)
            .OrderByDescending(ts => ts.StartedAtUtc);

        if (page != null || pageSize != null)
        {
            if (page == null || page == 0) page = 1;
            if (pageSize == null || pageSize == 0) pageSize = 10;
            query = query.Skip((page.Value - 1) *
                               pageSize.Value).Take(pageSize.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<int> GetCountAsync(string? procedureId, DateTime? fromUtc, DateTime? toUtc)
    {
        return await Filter(procedureId, fromUtc, toUtc).CountAsync();
    }

    private IQueryable<TrainingSession> Filter(string? procedureId, DateTime? fromUtc, DateTime? toUtc)
    {
        IQueryable<TrainingSession> query = _context.TrainingSessions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(procedureId))
            query = query.Where(ts => ts.ProcedureId == procedureId);
        if (fromUtc != null)
            query = query.Where(ts => ts.StartedAtUtc >= fromUtc.Value);
        if (toUtc != null)
            query = query.Where(ts => ts.StartedAtUtc <= toUtc.Value);

        return query;
    }
}
