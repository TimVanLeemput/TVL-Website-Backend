namespace VotingPoll.Core.Entities.Training;

public class TrainingSessionStep
{
    public int Id { get; set; }
    public int TrainingSessionId { get; set; }
    public TrainingSession TrainingSession { get; set; } = null!;

    public int Index { get; set; }
    public string Instruction { get; set; } = string.Empty;
    public double DurationSeconds { get; set; }
    public bool Completed { get; set; }
}
