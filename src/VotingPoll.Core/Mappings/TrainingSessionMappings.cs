using System.Globalization;
using VotingPoll.Core.Entities.Training;
using VotingPoll.Core.Models.DTOs;

namespace VotingPoll.Core.Mappings;

public static class TrainingSessionMappings
{
    /// <summary>Wire timestamps are ISO-8601 strings; parsed to UTC here (validator guarantees they parse).</summary>
    public static DateTime ParseUtc(string isoTimestamp)
    {
        return DateTime.Parse(isoTimestamp, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
    }

    public static TrainingSession ToEntity(this CreateTrainingSessionDto dto, DateTime receivedAtUtc)
    {
        TrainingSession session = new TrainingSession
        {
            SessionId = Guid.Parse(dto.SessionId),
            SchemaVersion = dto.SchemaVersion,
            ProcedureId = dto.ProcedureId,
            ProcedureTitle = dto.ProcedureTitle,
            StartedAtUtc = ParseUtc(dto.StartedAtUtc),
            CompletedAtUtc = ParseUtc(dto.CompletedAtUtc),
            DurationSeconds = dto.DurationSeconds,
            Score = dto.Score,
            Passed = dto.Passed,
            PassingScore = dto.PassingScore,
            DeviceModel = dto.Device.Model,
            DeviceOperatingSystem = dto.Device.OperatingSystem,
            AppVersion = dto.Device.AppVersion,
            ReceivedAtUtc = receivedAtUtc,
            Steps = dto.Steps.Select(step => new TrainingSessionStep
            {
                Index = step.Index,
                Instruction = step.Instruction,
                DurationSeconds = step.DurationSeconds,
                Completed = step.Completed,
            }).ToList(),
            Errors = dto.Errors.Select(error => new TrainingSessionError
            {
                StepIndex = error.StepIndex,
                RuleType = error.RuleType,
                Severity = error.Severity,
                ScoreDeduction = error.ScoreDeduction,
                AtSeconds = error.AtSeconds,
            }).ToList(),
        };
        return session;
    }

    public static TrainingSessionSummaryDto ToSummaryDto(this TrainingSession session)
    {
        TrainingSessionSummaryDto summaryDto = new TrainingSessionSummaryDto();
        FillSummary(session, summaryDto);
        return summaryDto;
    }

    public static TrainingSessionDetailDto ToDetailDto(this TrainingSession session)
    {
        TrainingSessionDetailDto detailDto = new TrainingSessionDetailDto
        {
            DeviceOperatingSystem = session.DeviceOperatingSystem,
            ReceivedAtUtc = session.ReceivedAtUtc,
            Steps = session.Steps.OrderBy(step => step.Index).Select(step => new TrainingStepDto
            {
                Index = step.Index,
                Instruction = step.Instruction,
                DurationSeconds = step.DurationSeconds,
                Completed = step.Completed,
            }).ToList(),
            Errors = session.Errors.OrderBy(error => error.AtSeconds).Select(error => new TrainingErrorDto
            {
                StepIndex = error.StepIndex,
                RuleType = error.RuleType,
                Severity = error.Severity,
                ScoreDeduction = error.ScoreDeduction,
                AtSeconds = error.AtSeconds,
            }).ToList(),
        };
        FillSummary(session, detailDto);
        return detailDto;
    }

    public static List<TrainingSessionSummaryDto> ToListOfSummaryDto(this List<TrainingSession> sessions)
    {
        return sessions.Select(session => session.ToSummaryDto()).ToList();
    }

    private static void FillSummary(TrainingSession session, TrainingSessionSummaryDto summaryDto)
    {
        summaryDto.Id = session.Id;
        summaryDto.SessionId = session.SessionId;
        summaryDto.ProcedureId = session.ProcedureId;
        summaryDto.ProcedureTitle = session.ProcedureTitle;
        summaryDto.StartedAtUtc = session.StartedAtUtc;
        summaryDto.CompletedAtUtc = session.CompletedAtUtc;
        summaryDto.DurationSeconds = session.DurationSeconds;
        summaryDto.Score = session.Score;
        summaryDto.Passed = session.Passed;
        summaryDto.PassingScore = session.PassingScore;
        summaryDto.ErrorCount = session.Errors.Count;
        summaryDto.DeviceModel = session.DeviceModel;
        summaryDto.AppVersion = session.AppVersion;
    }
}
