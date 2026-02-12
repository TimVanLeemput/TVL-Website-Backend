namespace VotingPoll.Core.Exceptions;

public class VoteNotFoundException : Exception
{
    public int VoteId { get; }
    
    public VoteNotFoundException()
    {
    }
    
    public VoteNotFoundException(int voteId)
        : base($"Vote {voteId} does not exist")
    {
        VoteId = voteId;
    }

    public VoteNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
    
}