namespace VotingPoll.API.Entities;

public class Poll
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TotalVotes { get; set; } = 0;
    public List<PollOption> AllPollOptions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosesAt { get; set; }
}