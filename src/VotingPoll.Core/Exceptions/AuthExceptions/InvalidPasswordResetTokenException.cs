namespace VotingPoll.Core.Exceptions.AuthExceptions;

public class InvalidPasswordResetTokenException() : Exception("Password reset link is invalid or has expired.");
