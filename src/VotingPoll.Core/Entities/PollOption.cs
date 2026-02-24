using VotingPoll.Core.Entities;

public class PollOption
{
    public int Id { get; set; }
    public string PollOptionName { get; set; } = string.Empty;
    public int PollId { get; set; }
    public Poll? Poll { get; set; }
    public List<Vote>? AllVotes { get; set; } = new List<Vote>(); 
    public int? TotalVotes { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
}