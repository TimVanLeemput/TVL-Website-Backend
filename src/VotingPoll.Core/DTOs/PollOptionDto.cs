namespace VotingPoll.Core.DTOs;

public class PollOptionDto
{
    public int Id { get; set; }
    public string PollOptionName { get; set; } = string.Empty;
    public int PollId { get; set; }
    public List<VoteDto>? AllVotes { get; set; }
    public int? TotalVotes { get; set; } = 0;
    public double VotesPercentage { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
}

public class CreatePollOptionDto
{
    public string PollOptionName { get; set; } = string.Empty;
}
