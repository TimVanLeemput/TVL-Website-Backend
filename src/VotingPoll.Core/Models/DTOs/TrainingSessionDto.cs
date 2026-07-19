namespace VotingPoll.Core.Models.DTOs;

// ---------- Incoming (the fixed TVL-VR Chapter 7 SessionResult wire contract) ----------
// Property names mirror the camelCase JSON emitted by Unity's JsonUtility; timestamps
// arrive as ISO-8601 strings and are parsed during mapping, not by the serializer.

public class CreateTrainingSessionDto
{
    public int SchemaVersion { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string ProcedureId { get; set; } = string.Empty;
    public string ProcedureTitle { get; set; } = string.Empty;
    public string StartedAtUtc { get; set; } = string.Empty;
    public string CompletedAtUtc { get; set; } = string.Empty;
    public double DurationSeconds { get; set; }
    public int Score { get; set; }
    public bool Passed { get; set; }
    public int PassingScore { get; set; }
    public List<CreateTrainingStepDto> Steps { get; set; } = new();
    public List<CreateTrainingErrorDto> Errors { get; set; } = new();
    public CreateTrainingDeviceDto Device { get; set; } = new();
}

public class CreateTrainingStepDto
{
    public int Index { get; set; }
    public string Instruction { get; set; } = string.Empty;
    public double DurationSeconds { get; set; }
    public bool Completed { get; set; }
}

public class CreateTrainingErrorDto
{
    public int StepIndex { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public int ScoreDeduction { get; set; }
    public double AtSeconds { get; set; }
}

public class CreateTrainingDeviceDto
{
    public string Model { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
}

// ---------- Outgoing (dashboard) ----------

public class TrainingSessionSummaryDto
{
    public int Id { get; set; }
    public Guid SessionId { get; set; }
    public string ProcedureId { get; set; } = string.Empty;
    public string ProcedureTitle { get; set; } = string.Empty;
    public DateTime StartedAtUtc { get; set; }
    public DateTime CompletedAtUtc { get; set; }
    public double DurationSeconds { get; set; }
    public int Score { get; set; }
    public bool Passed { get; set; }
    public int PassingScore { get; set; }
    public int ErrorCount { get; set; }
    public string DeviceModel { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;
}

public class TrainingSessionDetailDto : TrainingSessionSummaryDto
{
    public string DeviceOperatingSystem { get; set; } = string.Empty;
    public DateTime ReceivedAtUtc { get; set; }
    public List<TrainingStepDto> Steps { get; set; } = new();
    public List<TrainingErrorDto> Errors { get; set; } = new();
}

public class TrainingStepDto
{
    public int Index { get; set; }
    public string Instruction { get; set; } = string.Empty;
    public double DurationSeconds { get; set; }
    public bool Completed { get; set; }
}

public class TrainingErrorDto
{
    public int StepIndex { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public int ScoreDeduction { get; set; }
    public double AtSeconds { get; set; }
}

/// <summary>Service-level create outcome: the summary plus whether a new row was inserted (drives 201 vs idempotent 200).</summary>
public class TrainingSessionCreateResult
{
    public TrainingSessionSummaryDto Session { get; set; } = new();
    public bool Created { get; set; }
}
