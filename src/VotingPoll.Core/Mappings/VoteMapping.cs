using VotingPoll.Core.DTOs;
using VotingPoll.Core.Entities;

namespace VotingPoll.Core.Mappings;

public static class VoteMapping
{
    public static Vote ToEntity(this CreateVoteDto createVoteDto, int pollId)
    {
        Vote vote = new Vote
        {
            PollId = pollId,
            PollOptionId = createVoteDto.PollOptionId,
            UserId = createVoteDto.UserId,
            VotedAt = DateTime.UtcNow,
        };
        return vote;
    }
}