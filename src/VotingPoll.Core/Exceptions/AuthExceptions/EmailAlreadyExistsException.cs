namespace VotingPoll.Core.Exceptions.AuthExceptions;

public class EmailAlreadyExistsException : Exception
{
    public string Email { get; }

    public EmailAlreadyExistsException(string email)
        : base($"User with e-mail: {email}, already exists")
    {
        Email = email;
    }
}