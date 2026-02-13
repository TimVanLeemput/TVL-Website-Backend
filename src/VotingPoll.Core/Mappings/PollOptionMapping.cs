using VotingPoll.Core.DTOs;

namespace VotingPoll.Core.Mappings;

public static class PollOptionMapping
{
    public static PollOptionDto ToPollOptionDto(this CreatePollOptionDto createPollOptionDto)
    {
        PollOptionDto pollOption = new PollOptionDto
        {
            PollOptionName = createPollOptionDto.PollOptionName,
            CreatedAt = DateTime.UtcNow
        };
        return pollOption;
    }

    public static PollOptionDto ToPollOptionDto(this PollOption? pollOption, int pollId)
    {
        PollOptionDto pollOptionDto = new PollOptionDto
        {
            PollId = pollId,
            PollOptionName = pollOption.PollOptionName,
            CreatedAt = DateTime.UtcNow
        };
        return pollOptionDto;
    }

    public static List<PollOptionDto> ToPollOptionsDto(this IEnumerable<PollOption> pollOptions)
    {
        List<PollOptionDto> pollOptionDtos = pollOptions.Select(x =>
            {
                if (x == null) return null;
                return new PollOptionDto
                {
                    Id = x.Id,
                    PollId = x.PollId,
                    CreatedAt = x.CreatedAt,
                    PollOptionName = x.PollOptionName
                };
            }
        ).ToList();
        return pollOptionDtos;
    }

    public static PollOption ToEntity(this CreatePollOptionDto createPollOptionDto, int pollId)
    {
        PollOption pollOption = new PollOption
        {
            PollId = pollId,
            PollOptionName = createPollOptionDto.PollOptionName,
            CreatedAt = DateTime.UtcNow
        };
        return pollOption;
    }
}