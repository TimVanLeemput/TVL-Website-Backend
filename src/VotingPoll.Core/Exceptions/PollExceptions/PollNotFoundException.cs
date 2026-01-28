namespace VotingPoll.Core.Exceptions;

public class PollNotFoundException : Exception
{
    public int PollId { get; }

    public PollNotFoundException()
        : base($"No poll found")
    {
    }

    public PollNotFoundException(int pollId)
        : base($"Poll {pollId} does not exist")
    {
        PollId = pollId;
    }
}