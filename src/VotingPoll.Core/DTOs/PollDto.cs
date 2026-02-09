namespace VotingPoll.Core.DTOs;

public class PollDto
{
    public int Id { get; set; }
    public string? Title { get; set; } = string.Empty;
    public int TotalVotes { get; set; } = 0;
    public List<PollOptionDto>? AllPollOptions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosesAt { get; set; }
}
#region Dto Request

public class  CreatePollDto
{
    public string Title { get; set; } = string.Empty;
}


#endregion