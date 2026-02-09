namespace VotingPoll.Core.DTOs;

public class VoteDto
{
    public int Id { get; set; }
    public int PollOptionId { get; set; }
    public string? UserId { get; set; }
    public DateTime VotedAt { get; set; }
    public int PollId { get; set; }
}