using Microsoft.Extensions.Logging;
using Moq;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Exceptions;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Core.Models.DTOs;
using VotingPoll.Core.Services;

namespace VotingPoll.Tests;

/// <summary>
///  To be able to see which methods are being tested in console:
///   dotnet test src/VotingPoll.Tests --logger "console;verbosity=detailed"
/// </summary>
public class VotingServiceTests
{
    private readonly Mock<IPollRepository> _pollRepoMock;
    private readonly Mock<IVoteRepository> _voteRepoMock;
    private readonly Mock<IPollOptionRepository> _pollOptionRepoMock;
    private readonly Mock<ILogger<VotingService>> _loggerMock;
    private readonly VotingService _sut; // System Under Test

    public VotingServiceTests()
    {
        _pollRepoMock = new Mock<IPollRepository>();
        _voteRepoMock = new Mock<IVoteRepository>();
        _pollOptionRepoMock = new Mock<IPollOptionRepository>();
        _loggerMock = new Mock<ILogger<VotingService>>();
        _sut = new VotingService(_loggerMock.Object, _voteRepoMock.Object, _pollRepoMock.Object,
            _pollOptionRepoMock.Object);
    }

    [Fact]
    public async Task CreateVote_PollNotFound_ThrowsPollNotFoundException()
    {
        // Arrange
        _pollRepoMock.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Poll?)null);

        // Act & Assert
        await Assert.ThrowsAsync<PollNotFoundException>(() =>
            _sut.Create(1, 999, new CreateVoteDto() { PollOptionId = 1 }));
        
        _voteRepoMock.Verify(r => r.CreateAsync(It.IsAny<Vote>()), Times.Never);
    }

    [Fact]
    public async Task CreateVote_UserAlreadyVoted_ThrowsAlreadyVotedException()
    {
        // Arrange
        Poll poll = new Poll
        {
            Id = 1,
            Title = "Test Poll",
            AllPollOptions = new List<PollOption>
            {
                new PollOption { Id = 1, PollOptionName = "Option A", PollId = 1 }
            },
            ClosesAt = DateTime.UtcNow.AddDays(1)
        };

        _pollRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(poll);
        _voteRepoMock.Setup(r => r.UserAlreadyVotedAsync(1, 1))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyVotedException>(() =>
            _sut.Create(1, 1, new CreateVoteDto() { PollOptionId = 1 }));
    }

    [Fact]
    public async Task CreateVote_ValidVote_CreatesVoteAndReturnsConfirmation()
    {
        // Arrange
        PollOption pollOption = new PollOption { Id = 1, PollOptionName = "Option A", PollId = 1 }; // Added this line 
        Poll poll = new Poll
        {
            Id = 1,
            Title = "Test Poll",
            AllPollOptions = new List<PollOption>
            {
                pollOption
            },
            ClosesAt = DateTime.UtcNow.AddDays(1)
        };
        _pollRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(poll);
        _pollOptionRepoMock.Setup(r => r.GetAsync(1, 1)).ReturnsAsync(pollOption); // Added this line 
        _voteRepoMock.Setup(r => r.UserAlreadyVotedAsync(1, 1)).ReturnsAsync(false);
        _voteRepoMock.Setup(r => r.CreateAsync(It.IsAny<Vote>()))
            .ReturnsAsync((Vote v) => v);

        // Act
        VoteConfirmationDto result = await _sut.Create(1, 1, new CreateVoteDto() { PollOptionId = 1 });

        // Assert
        Assert.Equal("Test Poll", result.PollTitle);
        Assert.Equal("Option A", result.PollOptionName);
        _voteRepoMock.Verify(r => r.CreateAsync(It.Is<Vote>(v =>
            v.PollId == 1 && v.PollOptionId == 1 && v.UserId == 1)), Times.Once);
    }


    [Fact]
    
    public async Task CreateVotes_ValidVotes_CreatesVoteAndReturnPercentage()
    {
        // Arrange
        List<PollOption> pollOptions = new List<PollOption>
        {
            new PollOption
            {
                Id = 1, PollOptionName = "Option A", PollId = 1
            },
            new PollOption
            {
                Id = 2, PollOptionName = "Option B", PollId = 1
            }
        }; // Added this line 
        Poll poll = new Poll
        {
            Id = 1,
            Title = "Test Poll",
            AllPollOptions = pollOptions,
            ClosesAt = DateTime.UtcNow.AddDays(1)
        };
        _pollRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(poll);
        _pollOptionRepoMock.Setup(r => r.GetAsync(1, 1)).ReturnsAsync(pollOptions.First); // Added this line 
        _pollOptionRepoMock.Setup(r => r.GetAsync(1, 2)).ReturnsAsync(pollOptions[1]); // Added this line 
        _voteRepoMock.Setup(r => r.UserAlreadyVotedAsync(1, 1)).ReturnsAsync(false);
        _voteRepoMock.Setup(r => r.CreateAsync(It.IsAny<Vote>()))
            .ReturnsAsync((Vote v) => v);

        // Act
        VoteConfirmationDto result1 = await _sut.Create(1, 1, new CreateVoteDto() { PollOptionId = 1 });
        VoteConfirmationDto result2 = await _sut.Create(1, 1, new CreateVoteDto() { PollOptionId = 2 });

        // Assert
        Assert.Equal("Test Poll", result1.PollTitle);
        Assert.Equal("Option A", result1.PollOptionName);
        Assert.Equal("Option B", result2.PollOptionName);
        _voteRepoMock.Verify(r => r.CreateAsync(It.Is<Vote>(v =>
            v.PollId == 1 && v.PollOptionId == 1 && v.UserId == 1)));
        _voteRepoMock.Verify(r => r.CreateAsync(It.Is<Vote>(v =>
            v.PollId == 1 && v.PollOptionId == 2 && v.UserId == 1)));
    }


    [Fact]
    public async Task CreateVote_PollClosed_ThrowsPollClosedException()
    {
        Poll poll = new Poll
        {
            Id = 1,
            Title = "Test Poll",
            ClosesAt = DateTime.UtcNow.AddDays(-1)
        };

        _pollRepoMock.Setup(rule => rule.GetByIdAsync(1)).ReturnsAsync((poll));
        await Assert.ThrowsAsync<PollClosedException>(() =>
            _sut.Create(1, 1, new CreateVoteDto { PollOptionId = 1 }));
    }

    [Fact]
    public async Task CreateVote__WrongPollOptionId_ThrowInValidPollOptionException()
    {
        PollOption pollOption = new PollOption { Id = 1, PollOptionName = "Option A", PollId = 1 };
        Poll poll = new Poll
        {
            Id = 1,
            Title = "Test Poll",
            AllPollOptions = new List<PollOption>
            {
                pollOption
            }
        };

        _pollRepoMock.Setup(rule => rule.GetByIdAsync(1)).ReturnsAsync(poll);

        await Assert.ThrowsAsync<InvalidPollOptionException>(() =>
            _sut.Create(1, 1, new CreateVoteDto { PollOptionId = 2 }));
    }

    [Fact]
    public async Task CreateVote_PollOptionIsNull_ThrowInValidPollOptionException()
    {
        PollOption pollOption = new PollOption { Id = 1, PollOptionName = "Option A", PollId = 1 };
        Poll poll = new Poll
        {
            Id = 1,
            Title = "Test Poll",
            AllPollOptions = new List<PollOption>
            {
                pollOption
            }
        };

        _pollRepoMock.Setup(rule => rule.GetByIdAsync(1)).ReturnsAsync(poll);

        await Assert.ThrowsAsync<InvalidPollOptionException>(() =>
            _sut.Create(1, 1, new CreateVoteDto()));
    }
}