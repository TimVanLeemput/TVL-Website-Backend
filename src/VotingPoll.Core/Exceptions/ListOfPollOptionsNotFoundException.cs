namespace VotingPoll.Core.Exceptions;

public class ListOfPollOptionsNotFoundException : Exception
{
    public int PollId { get; }

    public ListOfPollOptionsNotFoundException()
        : base($"No list of poll options found")
    {
    }

    public ListOfPollOptionsNotFoundException(int pollId)
        : base($"List of poll options {pollId} does not exist or is empty")
    {
        PollId = pollId;
    }
}