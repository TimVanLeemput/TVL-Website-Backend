namespace VotingPoll.Core.Entities.Training;

public class TrainingSession
{
    public int Id { get; set; }

    /// <summary>Client-generated GUID from the headset — the idempotency key (unique index).</summary>
    public Guid SessionId { get; set; }

    public int SchemaVersion { get; set; }
    public string ProcedureId { get; set; } = string.Empty;
    public string ProcedureTitle { get; set; } = string.Empty;

    public DateTime StartedAtUtc { get; set; }
    public DateTime CompletedAtUtc { get; set; }
    public double DurationSeconds { get; set; }

    public int Score { get; set; }
    public bool Passed { get; set; }
    public int PassingScore { get; set; }

    public string DeviceModel { get; set; } = string.Empty;
    public string DeviceOperatingSystem { get; set; } = string.Empty;
    public string AppVersion { get; set; } = string.Empty;

    /// <summary>Server-side receive time — device clocks drift.</summary>
    public DateTime ReceivedAtUtc { get; set; }

    public List<TrainingSessionStep> Steps { get; set; } = new();
    public List<TrainingSessionError> Errors { get; set; } = new();
}
