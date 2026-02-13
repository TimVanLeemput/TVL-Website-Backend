namespace VotingPoll.Core.DTOs;

#region Dto Response

public class PollDto
{
    public int PollId { get; set; }
    public string? Title { get; set; } = string.Empty;
    public int TotalVotes { get; set; } = 0;
    public List<PollOptionDto>? AllPollOptions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosesAt { get; set; }
}

public class PollCreationDateDto
{
    public int PollId { get; set; }
    public string? Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PollResultsDto
{
    public int PollId { get; set; }
    public string? Title { get; set; } = string.Empty;
    public int TotalVotes { get; set; } = 0;
    public List<PollOptionDto>? AllPollOptions { get; set; }
}

#endregion

#region Dto Request

public class CreatePollDto
{
    public string Title { get; set; } = string.Empty;
    public List<CreatePollOptionDto>? AllPollOptions { get; set; }
}

public class UpdatePollDto
{
    public string Title { get; set; }
    public DateTime ClosesAt { get; set; }
}

#endregion