using VotingPoll.Core.Models;
using VotingPoll.Core.Models.DTOs;

namespace VotingPoll.Core.Interfaces.ServicesInterfaces;

public interface IPollService
{
    public Task<PagedList<PollDto>> GetAll(bool? isOpen = null, int? page = null, int? pageSize = null);
    public Task<PollDto> GetCurrentPollAsync();
    public Task<PollDto> GetById(int id);
    public Task<PollCreationDateDto> GetPollCreationDateById(int id);
    public Task<PollResultsDto> GetPollResultsById(int id);
    public Task<PollDto> Create(CreatePollDto createPollDto);
    public Task<PollDto> UpdatePoll(int id, UpdatePollDto? pollIn);
    public Task DeletePoll(int id);
}