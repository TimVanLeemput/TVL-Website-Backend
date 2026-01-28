using Microsoft.Extensions.Logging;
using Moq;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Core.Models.DTOs;
using VotingPoll.Core.Services;

namespace VotingPoll.Tests;

public class PollServiceTest
{
    private readonly Mock<IPollRepository> _pollRepoMock;
    private readonly Mock<ILogger<PollService>>? _pollServiceLoggerMock;
    private readonly PollService _sut;

    public PollServiceTest()
    {
        _pollRepoMock = new Mock<IPollRepository>();
        _sut = new PollService(_pollServiceLoggerMock?.Object, _pollRepoMock.Object);
    }

    [Fact]
    public async Task GetPollResults_ReturnsCorrectPercentages()
    {
        Poll poll = new Poll
        {
            Id = 1,
            Title = "Test Poll",
            AllPollOptions = new List<PollOption>
            {
                new PollOption
                {
                    Id = 1,
                    PollOptionName = "Option A",
                    AllVotes = new List<Vote> { new Vote { UserId = 0 }, new Vote { UserId = 1 }, new Vote { UserId = 2 } } // 3 votes
                },
                new PollOption
                {
                    Id = 2,
                    PollOptionName = "Option B",
                    AllVotes = new List<Vote> { new Vote { UserId = 3 } } // 1 vote
                }
            }
        };
        _pollRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(poll);

        PollResultsDto result = await _sut.GetPollResultsById(1);

        Assert.Equal(75.0, result?.AllPollOptions?[0].VotesPercentage);
        Assert.Equal(25.0, result?.AllPollOptions?[1].VotesPercentage);
        Assert.Equal(4, result?.TotalVotes);
    }
}