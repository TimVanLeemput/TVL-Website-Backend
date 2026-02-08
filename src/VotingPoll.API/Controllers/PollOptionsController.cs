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

    [HttpGet]
    public async Task<ActionResult<List<PollOption>>> GetAllOptionsForPoll(int id)
    {
        List<PollOption> pollOptions =
            await _context.PollOptions.Where(o => o.PollId == id).ToListAsync();
        return Ok(pollOptions);
    }
}