using Microsoft.Extensions.Logging;
using VotingPoll.Core.DTOs;
using VotingPoll.Core.Entities;

namespace VotingPoll.Core.Mappings;

public static class PollMappings
{
    public static PollDto ToDto(this Poll poll)
    {
        List<PollOptionDto>? optionDto = poll.AllPollOptions?.Select(options =>
            new PollOptionDto
            {
                Id = options.Id,
                PollOptionName = options.PollOptionName,
                PollId = options.PollId,
                TotalVotes = options.TotalVotes,
                CreatedAt = options.CreatedAt
            }).ToList();

        return new PollDto
        {
            PollId = poll.Id,
            Title = poll.Title,
            TotalVotes = poll.TotalVotes,
            AllPollOptions = optionDto,
            CreatedAt = poll.CreatedAt,
            ClosesAt = poll.ClosesAt,
        };
    }

    public static PollOptionDto ToPollOptionDto(this PollOptionDto pollOptionDto)
    {
        PollOptionDto pollOption = new PollOptionDto
        {
            PollOptionName = pollOptionDto.PollOptionName,
            PollId = pollOptionDto.PollId,
            CreatedAt = DateTime.UtcNow
        };
        return pollOption;
    }
    
    public static VoteConfirmationDto ToVoteConfirmationDto(this Poll poll,
        PollOption pollOption)
    {
        VoteConfirmationDto voteConfirmationDto = new VoteConfirmationDto
        {
            PollTitle = poll.Title,
            PollOptionName = pollOption.PollOptionName,
            VotedAt = DateTime.UtcNow
        };
        return voteConfirmationDto;
    }

    public static void ApplyTo(this UpdatePollDto dto, Poll poll)
    {
        poll.Title = dto.Title;
        poll.ClosesAt = dto.ClosesAt;
    }

    public static Poll ToEntity(this CreatePollDto createPollDto)
    {
        Poll createdPoll = new Poll
        {
            Title = createPollDto.Title,
            AllPollOptions = createPollDto.AllPollOptions?.Select(o => new PollOption
            {
                PollOptionName = o.PollOptionName,
                CreatedAt = DateTime.UtcNow
            }).ToList(),
            CreatedAt = DateTime.UtcNow,
        };
        return createdPoll;
    }
}