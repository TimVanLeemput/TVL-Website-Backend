namespace VotingPoll.Core.Exceptions.AuthExceptions;

public class InvalidVerificationTokenException : Exception
{
    public InvalidVerificationTokenException()
        : base("Verification link is invalid or has expired.")
    {
    }
}
