using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingPoll.API.Data;
using VotingPoll.API.Entities;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PollsController : ControllerBase
{
    AppDbContext _context;

    public PollsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Poll>>> GetAll()
    {
        List<Poll> polls = await _context.Polls.ToListAsync();
        return Ok(polls);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Poll>> GetById(int id)
    {
        Poll? pollToGet = await _context.Polls.FindAsync(id);
        if (pollToGet == null) return NotFound();
        return Ok(pollToGet);
    }

    [HttpPost]
    public async Task<ActionResult<Poll>> Create(CreatePollRequest request)
    {
        Poll createdPoll = new Poll
        {
            Title = request.Title,
            TotalVotes = 0,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Polls.AddAsync(createdPoll);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = createdPoll.Id }, createdPoll);
    }

    [HttpPost("{id}")]
    public async Task<ActionResult<PollOption>> AddPollOption(int id, CreatePollOptionRequest
        request)
    {
        Poll? poll = await _context.Polls.FindAsync(id);
        if (poll == null) return NotFound();

        PollOption pollOption = new PollOption
        {
            PollOptionName = request.PollOptionName,
            PollId = id,
            CreatedAt = DateTime.UtcNow
        };

        _context.PollOptions.Add(pollOption);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = pollOption.Id }, pollOption);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Poll>> Delete(int id)
    {
        Poll pollToDelete = await _context.Polls.FindAsync(id);
        if (pollToDelete == null) return NotFound();
        _context.Polls.Remove(pollToDelete);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Poll>> UpdatePoll(int id, Poll? pollIn)
    {
        Poll? pollToUpdate = await _context.Polls.FindAsync(id);
        if (pollToUpdate == null) return NotFound();
        if (pollIn != null)
        {
            pollToUpdate.Title = pollIn.Title;
            pollToUpdate.TotalVotes = pollIn.TotalVotes;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class CreatePollRequest
{
    public string Title { get; set; } = string.Empty;
}

public class CreatePollOptionRequest
{
    public string PollOptionName { get; set; } = string.Empty;
}