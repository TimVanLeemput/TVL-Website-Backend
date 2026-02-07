using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.Entities;
using VotingPoll.Infrastructure.Data;

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

    #region GET

    [HttpGet]
    public async Task<ActionResult<List<Poll>>> GetAll()
    {
        List<Poll> polls = await _context.Polls.ToListAsync();
        return Ok(polls);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Poll>> GetById(int id)
    {
        // Poll? pollToGet = await _context.Polls.FindAsync(id);
        Poll? pollToGet =
            await _context.Polls
                .Include(x => x.AllPollOptions)!
                .ThenInclude(x => x.AllVotes)
                .FirstOrDefaultAsync(x => x.Id == id);
        if (pollToGet == null) return NotFound();
        return Ok(pollToGet);
    }

    #endregion

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

    [HttpPost("{id}/options")]
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
        Poll pollToDelete = await _context.Polls.FindAsync(id) ?? throw new InvalidOperationException();
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

    [HttpGet("{id}/options/votes")]
    public async Task<ActionResult<List<PollOption>>> GetAllVotesForPollOption(int id)
    {
        // List<Vote> allVotesForPollOption = await _context.Polls
        //     .Where(x => x.Id == id)
        //     .SelectMany(x => x.AllPollOptions)
        //     .SelectMany(x => x.AllVotes)
        //     .ToListAsync();
        Poll? poll = await _context.Polls
            .Include(x => x.AllPollOptions)!
            .ThenInclude(x => x.AllVotes)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (poll == null) return NotFound();

        List<VoteCountPerOptionResponse> responses = (poll.AllPollOptions ?? new List<PollOption>())
            .Select(option => new VoteCountPerOptionResponse
            {
                PollOptionName = option.PollOptionName,
                TotalVotes = option.AllVotes?.Count ?? 0
            })
            .ToList();

        return Ok(responses);
    }
}

public class VoteCountPerOptionResponse
{
    public string PollOptionName { get; set; } = string.Empty;
    public int TotalVotes { get; set; }
}

public class CreatePollRequest
{
    public string Title { get; set; } = string.Empty;
}

public class CreatePollOptionRequest
{
    public string PollOptionName { get; set; } = string.Empty;
}