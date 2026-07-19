namespace VotingPoll.Core.Exceptions;

public class TrainingSessionNotFoundException : Exception
{
    public int TrainingSessionId { get; }

    public TrainingSessionNotFoundException(int trainingSessionId)
        : base($"Training session {trainingSessionId} does not exist")
    {
        TrainingSessionId = trainingSessionId;
    }
}
