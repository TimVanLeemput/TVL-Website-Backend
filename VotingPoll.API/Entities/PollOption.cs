namespace VotingPoll.API.Entities;

public class PollOption
{
    public int Id { get; set; }
    public string PollOptionName { get; set; } = string.Empty;
    public int PollId { get; set; }
    public Poll Poll { get; set; }
    public List<Vote>? AllVotes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? ClosesAt { get; set; }
}