namespace VotingPoll.Core.Models.DTOs;

public class VoteDto
{
    public int Id { get; set; }
    public int PollOptionId { get; set; }
    public string? UserId { get; set; }
    public DateTime VotedAt { get; set; }
    public int PollId { get; set; }
}

public class CreateVoteDto
{
    public string UserId { get; set; } = string.Empty;
    public int PollOptionId { get; set; }
}

public class VoteConfirmationDto
{
    public string? PollTitle { get; set; }  = string.Empty;
    public string? PollOptionName { get; set; } = string.Empty;
    public DateTime VotedAt { get; set; }
}