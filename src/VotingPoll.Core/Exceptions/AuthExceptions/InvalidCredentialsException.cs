namespace VotingPoll.Core.Exceptions.AuthExceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base($"Invalid credentials - Your email or password is incorrect")
    {
    }
}