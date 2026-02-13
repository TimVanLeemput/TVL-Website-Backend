using VotingPoll.Core.DTOs;

namespace VotingPoll.Core.Interfaces.ServicesInterfaces;

public interface IPollService
{
    public Task<List<PollDto>> GetAll();
    public Task<PollDto> GetById(int id);
    public Task<PollCreationDateDto> GetPollCreationDateById(int id);
    public Task<PollResultsDto> GetPollResultsById(int id);
    public Task<PollDto> Create(CreatePollDto createPollDto);
    public Task<PollDto> UpdatePoll(int id, UpdatePollDto? pollIn);
    public Task DeletePoll(int id);
}