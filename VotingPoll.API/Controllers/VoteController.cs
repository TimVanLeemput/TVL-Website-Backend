using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using VotingPoll.API.Data;
using VotingPoll.API.Entities;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/polls/{id}/options/{optionId}/votes")]
public class VoteController: ControllerBase
{
    private readonly AppDbContext _context;
    public VoteController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Vote>>> GetAllVotesForPollOption(PollOption pollOption)
    {
       List<Vote> allVotes = await _context.PollOptions
           .Where(x => x.Id == pollOption.Id)
           .SelectMany(y => y.AllVotes)
           .ToListAsync();
       return Ok(allVotes);
    }
    
    
}