using Microsoft.Extensions.Logging;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Exceptions;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Mappings;
using VotingPoll.Core.Models;
using VotingPoll.Core.Models.DTOs;

namespace VotingPoll.Core.Services;

public class PollService : IPollService
{
    // todo create a method to GetAllPollsWithAllOptionsAndAllVotesPercentage etc.. all in one call

    private readonly ILogger<PollService> _logger;
    private readonly IPollRepository _pollRepository;

    public PollService(ILogger<PollService>? logger, IPollRepository pollRepository)
    {
        _pollRepository = pollRepository;
        _logger = logger;
    }

    public async Task<PagedList<PollDto>> GetAll(bool? isOpen = null, int? page = null, int? pageSize = null)
    {
        int totalCount = await _pollRepository.GetAllPollsCountAsync(isOpen);

        List<Poll> polls = await _pollRepository.GetAllAsync(isOpen, page, pageSize);

        List<PollDto> listOfPolLDtos = polls.ToListOfPollDtos(true);
        PagedList<PollDto> pagedPollList = new PagedList<PollDto>(listOfPolLDtos, totalCount, page, pageSize);

        return pagedPollList;
    }

    public async Task<PollDto> GetById(int id)
    {
        Poll pollToGet = await GetPollOrThrow(id);

        PollDto pollDto = pollToGet.ToDto();
        return pollDto;
    }

    public async Task<PollCreationDateDto> GetPollCreationDateById(int id)
    {
        Poll poll = await GetPollOrThrow(id);

        return poll.ToPollCreationDateDto();
    }

    public async Task<PollResultsDto> GetPollResultsById(int id)
    {
        Poll poll = await GetPollOrThrow(id);
        PollResultsDto pollResultsDt = poll.ToPollResultsDto();
        return pollResultsDt;
    }

    public async Task<PollDto> Create(CreatePollDto createPollDto)
    {
        Poll createdPoll = createPollDto.ToEntity();

        await _pollRepository.CreateAsync(createdPoll);
        PollDto createdPollDto = createdPoll.ToDto();

        return createdPollDto;
    }

    public async Task<PollDto> UpdatePoll(int id, UpdatePollDto? pollIn)
    {
        Poll? pollToUpdate = await _pollRepository.GetByIdAsync(id);
        if (pollToUpdate == null) throw new PollNotFoundException(id);

        pollIn?.ApplyTo(pollToUpdate);

        await _pollRepository.UpdatePoll();
        PollDto pollDto = pollToUpdate.ToDto();
        return pollDto;
    }

    private async Task<Poll> GetPollOrThrow(int id)
    {
        Poll? pollToGet = await _pollRepository.GetByIdAsync(id);
        if (pollToGet == null) throw new PollNotFoundException(id);
        return pollToGet;
    }

    public async Task DeletePoll(int id)
    {
        if (!await _pollRepository.ExistsAsync(id))
            throw new PollNotFoundException(id);

        await _pollRepository.DeleteAsync(id);
    }
}