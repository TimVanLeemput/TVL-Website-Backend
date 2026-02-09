using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VotingPoll.Infrastructure.Data;
using VotingPoll.Core.Entities;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/polls/{pollId}/vote")]
public class VoteController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<VoteController> _logger;

    public VoteController(AppDbContext context, ILogger<VoteController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Vote>>> GetAllVotesForPoll(int pollId)
    {
        List<Vote> votes = await _context.Votes
            .Where(v => v.PollId == pollId)
            .ToListAsync();
        return Ok(votes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Vote>> GetById(int id)
    {
        Vote? voteToGet = await _context.Votes.FindAsync(id);
        if (voteToGet == null) return NotFound();
        return Ok(voteToGet);
    }


    [HttpPost]
    public async Task<ActionResult<Vote>> Create(int pollId, CreateVoteRequest request)
    {
        // Verify poll exists
        Poll? poll = await _context.Polls.FindAsync(pollId);
        if (poll == null) return NotFound("Poll not found");

        Vote createdVote = new Vote
        {
            UserId = request.UserId,
            PollId = pollId, // From route
            PollOptionId = request.PollOptionId, // From body
            PollOption = await _context.PollOptions.FindAsync(request.PollOptionId),
            VotedAt = DateTime.UtcNow
        };

        await _context.Votes.AddAsync(createdVote);

        createdVote.Poll.TotalVotes++;
        if (createdVote.PollOption != null)
        {
            createdVote.PollOption.TotalVotes++;
            _logger.LogInformation($"Vote added to poll option {createdVote.PollOption.Id}");
            createdVote.PollOption.AllVotes?.Add(createdVote);
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { pollId = pollId, id = createdVote.Id },
            createdVote);
    }

    public class CreateVoteRequest
    {
        public string UserId { get; set; } = "Tim -- " + Guid.NewGuid();
        public int PollOptionId { get; set; }
    }
}