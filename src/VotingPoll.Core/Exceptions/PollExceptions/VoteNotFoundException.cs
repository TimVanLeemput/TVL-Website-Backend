namespace VotingPoll.Core.Exceptions;

public class VoteNotFoundException : Exception
{
    public int VoteId { get; }
    
    public VoteNotFoundException(int voteId)
        : base($"Vote {voteId} does not exist")
    {
        VoteId = voteId;
    }
}