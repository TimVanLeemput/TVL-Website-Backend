using Microsoft.AspNetCore.Mvc;
using VotingPoll.Core.DTOs;
using VotingPoll.Core.Entities;
using VotingPoll.Infrastructure.Repositories;

namespace VotingPoll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PollsController : ControllerBase
{
    IPollRepository _pollRepository;
    IPollOptionRepository _pollOptionRepository;

    public PollsController(IPollRepository pollRepository,
        IPollOptionRepository pollOptionRepository)
    {
        _pollRepository = pollRepository;
        _pollOptionRepository = pollOptionRepository;
    }

    #region GET

    [HttpGet]
    public async Task<ActionResult<List<PollDto>>> GetAll()
    {
        List<Poll> polls = await _pollRepository.GetAllAsync();
        List<PollDto> pollDtos = polls.Select(poll => new PollDto
        {
            Id = poll.Id,
            Title = poll.Title,
            TotalVotes = poll.TotalVotes,
            CreatedAt = poll.CreatedAt,
            ClosesAt = poll.ClosesAt
        }).ToList();
        return Ok(pollDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PollDto>> GetById(int id)
    {
        Poll? pollToGet = await _pollRepository.GetByIdAsync(id);
        if (pollToGet == null) return NotFound();
        List<PollOptionDto>? optionDtos = pollToGet.AllPollOptions!.Select(options =>
            new PollOptionDto
            {
                Id = options.Id,
                PollOptionName = options.PollOptionName,
                PollId = options.PollId,
                TotalVotes = options.TotalVotes,
                CreatedAt = options.CreatedAt
            }).ToList();

        PollDto pollDto = new PollDto
        {
            Id = pollToGet.Id,
            Title = pollToGet.Title,
            TotalVotes = pollToGet.TotalVotes,
            AllPollOptions = optionDtos,
            CreatedAt = pollToGet.CreatedAt,
            ClosesAt = pollToGet.ClosesAt,
        };
        return Ok(pollDto);
    }

    #endregion

    #region Create

    [HttpPost]
    public async Task<ActionResult<PollDto>> Create(CreatePollDto createPollDto)
    {
        Poll createdPoll = new Poll
        {
            Title = createPollDto.Title,
            CreatedAt = DateTime.UtcNow,
        };

        await _pollRepository.CreateAsync(createdPoll);

        PollDto createdPollDto = new PollDto
        {
            Id = createdPoll.Id,
            Title = createdPoll.Title,
            TotalVotes = createdPoll.TotalVotes,
            CreatedAt = createdPoll.CreatedAt,
            ClosesAt = createdPoll.ClosesAt,
        };

        return CreatedAtAction(nameof(GetById), new { id = createdPoll.Id }, createdPollDto);
    }

    [HttpPost("{id}/options")]
    public async Task<ActionResult<PollOptionDto>> CreatePollOption(int id, PollOptionDto pollOptionDto)
    {
        Poll? poll = await _pollRepository.GetByIdAsync(id);
        if (poll == null) return NotFound();

        PollOptionDto pollOption = new PollOptionDto
        {
            PollOptionName = pollOptionDto.PollOptionName,
            PollId = id,
            CreatedAt = DateTime.UtcNow
        };

        // _context.PollOptions.Add(pollOption);
        // await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = pollOption.Id }, pollOption);
    }

    #endregion

    #region Update

    // [HttpPut("{id}")]
    // public async Task<ActionResult<Poll>> UpdatePoll(int id, Poll? pollIn)
    // {
    //     Poll? pollToUpdate = await _context.Polls.FindAsync(id);
    //     if (pollToUpdate == null) return NotFound();
    //     if (pollIn != null)
    //     {
    //         pollToUpdate.Title = pollIn.Title;
    //         pollToUpdate.TotalVotes = pollIn.TotalVotes;
    //     }
    //
    //     await _context.SaveChangesAsync();
    //     return NoContent();
    // }

    #endregion


    #region Delete

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        if (!await _pollRepository.ExistsAsync(id)) return NotFound();

        await _pollRepository.DeleteAsync(id);
        return NoContent();
    }

    #endregion
}