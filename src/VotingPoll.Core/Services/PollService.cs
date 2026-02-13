using VotingPoll.Core.DTOs;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Exceptions;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Mappings;
using VotingPoll.Infrastructure.Repositories;

namespace VotingPoll.Core.Services;

public class PollService : IPollService
{
    private readonly IPollOptionRepository _pollOptionRepository;
    private readonly IPollRepository _pollRepository;

    public PollService(
        IPollOptionRepository pollOptionRepository, IPollRepository pollRepository)
    {
        _pollOptionRepository = pollOptionRepository;
        _pollRepository = pollRepository;
    }

    public async Task<List<PollDto>> GetAll()
    {
        List<Poll> polls = await _pollRepository.GetAllAsync();
        if (polls.Count == 0)
            throw new PollNotFoundException();

        List<PollDto> listOfPolLDtos = polls.ToListOfPollDtos();

        return listOfPolLDtos;
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

    public async Task<PollDto> Create(CreatePollDto createPollDto)
    {
        Poll createdPoll = createPollDto.ToEntity();

        await _pollRepository.CreateAsync(createdPoll);
        PollDto createdPollDto = createdPoll.ToDto();

        return createdPollDto;
    }

    public async Task<PollOptionDto> CreatePollOption(int id, CreatePollOptionDto createPollOptionDto)
    {
        await GetPollOrThrow(id);

        PollOption pollOption = createPollOptionDto.ToEntity(id);
        await _pollOptionRepository.CreateAsync(pollOption);
        PollOptionDto pollOptionDto = pollOption.ToPollOptionDto(id);

        return pollOptionDto;
    }

    public async Task<PollDto> UpdatePoll(int id, UpdatePollDto? pollIn)
    {
        Poll? pollToUpdate = await _pollRepository.GetByIdAsync(id);
        if (pollToUpdate == null) throw new PollNotFoundException(id);

        pollIn?.ApplyTo(pollToUpdate);

        if (pollIn?.ClosesAt < DateTime.UtcNow)
            throw new PollClosedException(id);

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