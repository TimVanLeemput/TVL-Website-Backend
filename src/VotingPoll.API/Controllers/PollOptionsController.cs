using Microsoft.AspNetCore.Mvc;
using VotingPoll.Core.DTOs;
using VotingPoll.Infrastructure.Repositories;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/polls/{id}/options")]
public class PollOptionsController : ControllerBase
{
    private readonly IPollOptionRepository _pollOptionRepository;

    public PollOptionsController(IPollOptionRepository pollOptionRepository)
    {
        _pollOptionRepository = pollOptionRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<PollOptionDto>>> GetAllOptionsForPoll(int id)
    {
        List<PollOption> pollOptions = await _pollOptionRepository.GetAllAsync(id);
        List<PollOptionDto> pollOptionDtos = pollOptions.Select(x => new PollOptionDto
        {
            Id = x.Id,
            PollId = x.PollId,
            CreatedAt = x.CreatedAt,
            PollOptionName = x.PollOptionName
        }).ToList();

        return Ok(pollOptionDtos);
    }

    [HttpGet("{pollOptionId}")]
    public async Task<ActionResult<PollOptionDto>> GetPollOption(int id, int pollOptionId)
    {
        PollOption pollOption = await _pollOptionRepository.GetAsync(pollOptionId);
        if (pollOption == null)
            return NotFound();
        if (pollOption.PollId != id)
            return NotFound();
        PollOptionDto pollOptionDto = new PollOptionDto
        {
            Id = pollOption.Id,
            PollOptionName = pollOption.PollOptionName,
            PollId = pollOption.PollId,
            TotalVotes = pollOption.TotalVotes,
            CreatedAt = pollOption.CreatedAt,
        };
        return Ok(pollOptionDto);
    }
}