namespace VotingPoll.Core.Exceptions.AuthExceptions;

public class EmailNotVerifiedException : Exception
{
    public EmailNotVerifiedException()
        : base("Please verify your email before logging in.")
    {
    }
}
