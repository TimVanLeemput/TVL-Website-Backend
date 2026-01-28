namespace VotingPoll.Core.Exceptions.AuthExceptions;

public class VerificationEmailCooldownException() : Exception("Please wait before requesting another verification email.");
