using Microsoft.AspNetCore.Mvc;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PollsController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<PollResponse>> GetAll()
    {
        // Hardcoded for now — database comes later
        List<PollResponse> polls = new List<PollResponse>
        {
            new PollResponse { Id = 1, Title = "Favorite Color", TotalVotes = 42 },
            new PollResponse { Id = 2, Title = "Best Programming Language", TotalVotes = 128 }
        };
        return Ok(polls);
    }
    

    [HttpGet("{id}")]
    public ActionResult<PollResponse> GetById(int id)
    {
        if (id == 1)
        {
            return Ok(new PollResponse { Id = 1, Title = "Favorite Color", TotalVotes = 42 });
        }

        if (id == 2)
        {
            return Ok(new PollResponse { Id = 2, Title = "Best Programming Language", TotalVotes = 128 });
        }

        return NotFound();
    }

    [HttpPost]
    public ActionResult<PollResponse> Create(CreatePollRequest request)
    {
        PollResponse created = new PollResponse
        {
            Id = 3,
            Title = request.Title,
            TotalVotes = 0
        };
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteById(int id)
    {
        if (id == 1) return NoContent();
        return NotFound();
    }
}

// Temporary classes — will become proper DTOs later
public class PollResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TotalVotes { get; set; }
}


public class CreatePollRequest
{
    public string Title { get; set; } = string.Empty;
}