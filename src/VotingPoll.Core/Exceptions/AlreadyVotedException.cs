namespace VotingPoll.Core.Exceptions;

public class AlreadyVotedException : Exception
{
    public string UserId { get; }

    public  AlreadyVotedException()
    {
    }
    
    public AlreadyVotedException(string userId)
        : base($"User {userId} has already voted")
    {
        UserId = userId;
    }

    public AlreadyVotedException(string message, Exception inner)
        : base(message, inner)
    {
    }

}