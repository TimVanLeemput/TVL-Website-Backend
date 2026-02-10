namespace VotingPoll.Core.Exceptions;

public class PollClosedException : Exception
{
    public int PollId { get; }
    
    public PollClosedException()
    {
    }
    
    public PollClosedException(int pollId)
        : base($"Poll {pollId} is closed for voting")
    {
        PollId = pollId;
    }

    public PollClosedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}