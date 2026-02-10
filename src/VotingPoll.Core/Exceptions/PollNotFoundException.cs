namespace VotingPoll.Core.Exceptions;

public class PollNotFoundException : Exception
{
    public int PollId { get; }
    
    public PollNotFoundException()
    {
    }
    
    public PollNotFoundException(int pollId)
        : base($"Poll {pollId} does not exist")
    {
        PollId = pollId;
    }

    public PollNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
    
}