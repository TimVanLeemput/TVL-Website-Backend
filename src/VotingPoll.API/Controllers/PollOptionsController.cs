using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.Entities;
using VotingPoll.Infrastructure.Data;

namespace VotingPoll.Core.Controllers;

[ApiController]
[Route("api/polls/{id}/options")]
public class PollOptionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PollOptionsController(AppDbContext context)
    {
        _context = context;
    }
    //
    [HttpGet]
    public async Task<ActionResult<List<PollOption>>> GetAllOptionsForPoll(Poll poll)
    {
        List<PollOption> pollOptions =
            await _context.Polls.Where(x => x.Id == poll.Id).SelectMany(x => x.AllPollOptions!).ToListAsync();
        return Ok(pollOptions);
    }
}