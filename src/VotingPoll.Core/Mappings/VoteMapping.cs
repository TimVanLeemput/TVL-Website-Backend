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

    public static VoteDto ToDto(this Vote vote)
    {
        VoteDto voteDto = new VoteDto
        {
            Id = vote.Id,
            PollOptionId = vote.PollOptionId,
            UserId = vote.UserId,
            VotedAt = vote.VotedAt,
            PollId = vote.PollId,
        };
        return voteDto;
    }

    public static List<VoteDto> ToListOfVotesDto(this List<Vote> votes)
    {
        List<VoteDto> votesDto = votes.Select(vote => new VoteDto
        {
            Id = vote.Id,
            PollOptionId = vote.PollOptionId,
            UserId = vote.UserId,
            PollId = vote.PollId,
            VotedAt = vote.VotedAt
        }).ToList();
        return votesDto;
    }
}