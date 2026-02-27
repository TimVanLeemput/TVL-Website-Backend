namespace VotingPoll.Core.Exceptions;

public class PollOptionNotFoundException : Exception
{
    public int PollOptionId { get; }

    public PollOptionNotFoundException()
        : base($"No poll option found")
    {
    }

    public PollOptionNotFoundException(int pollOptionId)
        : base($"Poll option {pollOptionId} does not exist")
    {
        PollOptionId = pollOptionId;
    }
}