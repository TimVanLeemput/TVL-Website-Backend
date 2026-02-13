using Microsoft.Extensions.Logging;
using VotingPoll.Core.DTOs;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Exceptions;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Mappings;

namespace VotingPoll.Core.Services;

public class PollOptionService : IPollOptionService
{
    private readonly ILogger<PollOptionService> _logger;
    private readonly IPollOptionRepository _pollOptionRepository;
    private readonly IPollRepository _pollRepository;

    public PollOptionService(IPollOptionRepository pollOptionRepository, IPollRepository pollRepository,
        ILogger<PollOptionService> logger)
    {
        _pollOptionRepository = pollOptionRepository;
        _pollRepository = pollRepository;
        _logger = logger;
    }

    public async Task<PollOptionDto> GetById(int pollId, int pollOptionId)
    {
        PollOption? pollOption = await _pollOptionRepository.GetAsync(pollId, pollOptionId);
        if (pollOption == null)
        {
            if (!await _pollRepository.ExistsAsync(pollId))
                throw new PollNotFoundException(pollId);
            throw new PollOptionNotFoundException(pollOptionId);
        }

        PollOptionDto pollOptionDto = pollOption.ToPollOptionDto(pollOptionId);
        return pollOptionDto;
    }

    public async Task<List<PollOptionDto>> GetAllPollOptionsForPoll(int pollId)
    {
        List<PollOption?> pollOptions = await _pollOptionRepository.GetAllPollOptionsForPollAsync(pollId);
        if (pollOptions == null || pollOptions.Count <= 0)
        {
            _logger.LogWarning($"Poll options are empty, checking if poll exists with id = {pollId}");
            if (!await _pollRepository.ExistsAsync(pollId))
                throw new PollNotFoundException(pollId);
            throw new ListOfPollOptionsNotFoundException(pollId);
        }

        List<PollOptionDto> pollOptionsDto = pollOptions.ToPollOptionsDto();
        return pollOptionsDto!;
    }

    public async Task<PollOptionDto> CreatePollOption(int id, CreatePollOptionDto createPollOptionDto)
    {
        await GetPollOrThrow(id);

        PollOption pollOption = createPollOptionDto.ToEntity(id);
        await _pollOptionRepository.CreateAsync(pollOption);
        PollOptionDto pollOptionDto = pollOption.ToPollOptionDto(id);

        return pollOptionDto;
    }

    public async Task<string> DeletePollOption(int pollId, int pollOptionId)
    {
        PollOption? pollOptionDto = await _pollOptionRepository.GetAsync(pollId, pollOptionId);
        if (pollOptionDto == null)
        {
            if (!await _pollRepository.ExistsAsync(pollId))
                throw new PollNotFoundException(pollId);
            throw new PollOptionNotFoundException(pollOptionId);
        }

        string? pollOptionName = pollOptionDto?.PollOptionName; // Expose which pollOption has been delete
        await _pollOptionRepository.DeleteAsync(pollId, pollOptionId);
        return pollOptionName;
    }

    private async Task<Poll> GetPollOrThrow(int id)
    {
        Poll? pollToGet = await _pollRepository.GetByIdAsync(id);
        if (pollToGet == null) throw new PollNotFoundException(id);
        return pollToGet;
    }
}