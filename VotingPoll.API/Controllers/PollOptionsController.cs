using Microsoft.AspNetCore.Mvc;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/polls/{id}/options")]
public class PollOptionsController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<PollResponse>> GetAllOptionsForPoll()
    {
        // Hardcoded for now — a database comes later
        List<PollOption> pollOptions = new List<PollOption>()
        {
            new PollOption { Id = 1, PollOptionName = "Red" },
            new PollOption { Id = 2, PollOptionName = "Blue" },
            new PollOption { Id = 3, PollOptionName = "Green" }
        };

        return Ok(pollOptions);
    }
}

public class PollOption
{
    public int Id { get; set; }
    public string PollOptionName { get; set; } = string.Empty;
}