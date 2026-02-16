using VotingPoll.Core.Entities;
using VotingPoll.Core.Exceptions;
using VotingPoll.Core.Models.DTOs;

namespace VotingPoll.Core.Mappings;

public static class PollMappings
{
    public static PollDto ToDto(this Poll poll)
    {
        var optionDtos = GetListOfPollOptionDtosWithVotingResults(poll);
        if (optionDtos == null)
            throw new ListOfPollOptionsNotFoundException();
        int totalVotes = (int)optionDtos.Sum(o => o.TotalVotes);

        return new PollDto
        {
            PollId = poll.Id,
            Title = poll.Title,
            TotalVotes = totalVotes,
            AllPollOptions = optionDtos,
            CreatedAt = poll.CreatedAt,
            ClosesAt = poll.ClosesAt,
        };
    }

    public static PollCreationDateDto ToPollCreationDateDto(this Poll poll)
    {
        PollCreationDateDto pollCreationDateDto = new PollCreationDateDto
        {
            PollId = poll.Id,
            Title = poll.Title,
            CreatedAt = poll.CreatedAt
        };
        return pollCreationDateDto;
    }

    public static VoteConfirmationDto ToVoteConfirmationDto(this Poll poll,
        PollOption? pollOption)
    {
        VoteConfirmationDto voteConfirmationDto = new VoteConfirmationDto
        {
            PollTitle = poll.Title,
            PollOptionName = pollOption?.PollOptionName,
            VotedAt = DateTime.UtcNow
        };
        return voteConfirmationDto;
    }

    public static List<PollDto> ToListOfPollDtos(this List<Poll> polls)
    {
        List<PollDto> listOfPollDtos = polls.Select(poll => new PollDto
        {
            Title = poll.Title,
            CreatedAt = poll.CreatedAt,
            ClosesAt = poll.ClosesAt
        }).ToList();
        return listOfPollDtos;
    }

    public static List<PollDto> ToListOfPollDtos(this List<Poll> polls, bool showPollOptions)
    {
        List<PollDto> listOfPollDtos = polls.Select(poll => new PollDto
        {
            Title = poll.Title,
            AllPollOptions = poll.AllPollOptions.ToPollOptionsDto(),
            CreatedAt = poll.CreatedAt,
            ClosesAt = poll.ClosesAt
        }).ToList();
        return listOfPollDtos;
    }


    public static PollResultsDto ToPollResultsDto(this Poll poll)
    {
        var optionDtos = GetListOfPollOptionDtosWithVotingResults(poll);
        int totalVotes = poll.AllPollOptions.Sum(o => o.AllVotes.Count);
        
        PollResultsDto pollResultsDto = new PollResultsDto()
        {
            PollId = poll.Id,
            Title = poll.Title,
            TotalVotes = totalVotes,
            AllPollOptions = optionDtos,
        };

        return pollResultsDto;
    }

    private static List<PollOptionDto>? GetListOfPollOptionDtosWithVotingResults(Poll poll)
    {
        int totalVotes = poll.AllPollOptions.Sum(o => o.AllVotes.Count);
        List<PollOptionDto>? optionDto = poll.AllPollOptions?.Select(options =>
            new PollOptionDto
            {
                Id = options.Id,
                PollOptionName = options.PollOptionName,
                PollId = options.PollId,
                TotalVotes = options.AllVotes.Count,
                VotesPercentage = totalVotes > 0 ? Math.Round((double)options.AllVotes.Count / totalVotes * 100, 1) : 0,
                CreatedAt = options.CreatedAt
            }).ToList();
        return optionDto;
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