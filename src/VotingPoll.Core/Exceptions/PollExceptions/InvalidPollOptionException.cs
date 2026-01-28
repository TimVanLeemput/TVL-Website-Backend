namespace VotingPoll.Core.Exceptions;

public class InvalidPollOptionException : Exception
{
    public int PollOptionId { get; }

    public InvalidPollOptionException() : base("PollOption cannot be null")
    {
    }

    public InvalidPollOptionException(int pollOptionId)
        : base($"PollOption does not belong to this poll {pollOptionId}")
    {
        PollOptionId = pollOptionId;
    }

}