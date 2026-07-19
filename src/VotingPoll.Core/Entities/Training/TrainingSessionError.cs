namespace VotingPoll.Core.Entities.Training;

public class TrainingSessionError
{
    public int Id { get; set; }
    public int TrainingSessionId { get; set; }
    public TrainingSession TrainingSession { get; set; } = null!;

    public int StepIndex { get; set; }
    public string RuleType { get; set; } = string.Empty;

    /// <summary>Stored as the wire string, not an enum — new rule severities must not bounce off the backend.</summary>
    public string Severity { get; set; } = string.Empty;

    public int ScoreDeduction { get; set; }
    public double AtSeconds { get; set; }
}
