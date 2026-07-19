using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using VotingPoll.Core.Entities.Training;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Core.Mappings;
using VotingPoll.Core.Models.DTOs;
using VotingPoll.Core.Services;

public class TrainingSessionServiceTests
{
    private static CreateTrainingSessionDto ValidDto(string sessionId)
    {
        return new CreateTrainingSessionDto
        {
            SchemaVersion = 1,
            SessionId = sessionId,
            ProcedureId = "gowning-v1",
            ProcedureTitle = "Cleanroom Gowning",
            StartedAtUtc = "2026-07-19T12:00:00Z",
            CompletedAtUtc = "2026-07-19T12:02:03.500Z",
            DurationSeconds = 123.5,
            Score = 75,
            Passed = false,
            PassingScore = 80,
            Steps = new List<CreateTrainingStepDto>
            {
                new CreateTrainingStepDto
                    { Index = 0, Instruction = "Sanitize", DurationSeconds = 5.2, Completed = true },
            },
            Errors = new List<CreateTrainingErrorDto>
            {
                new CreateTrainingErrorDto
                {
                    StepIndex = 0, RuleType = "ContaminationTouchRule", Severity = "Critical",
                    ScoreDeduction = 25, AtSeconds = 30.1
                },
            },
            Device = new CreateTrainingDeviceDto
                { Model = "Quest 3", OperatingSystem = "Android 12", AppVersion = "1.0" },
        };
    }

    [Fact]
    public void ToEntity_ParsesTimestampsAsUtc_AndMapsChildren()
    {
        DateTime receivedAt = new DateTime(2026, 7, 19, 13, 0, 0, DateTimeKind.Utc);

        TrainingSession entity = ValidDto(Guid.NewGuid().ToString()).ToEntity(receivedAt);

        Assert.Equal(new DateTime(2026, 7, 19, 12, 0, 0, DateTimeKind.Utc), entity.StartedAtUtc);
        Assert.Equal(new DateTime(2026, 7, 19, 12, 2, 3, 500, DateTimeKind.Utc), entity.CompletedAtUtc);
        Assert.Equal(receivedAt, entity.ReceivedAtUtc);
        Assert.Single(entity.Steps);
        Assert.Equal("Sanitize", entity.Steps[0].Instruction);
        Assert.Single(entity.Errors);
        Assert.Equal("Critical", entity.Errors[0].Severity);
        Assert.Equal("Quest 3", entity.DeviceModel);
    }

    [Fact]
    public void ToDetailDto_RoundTripsAllFields()
    {
        TrainingSession entity = ValidDto(Guid.NewGuid().ToString()).ToEntity(DateTime.UtcNow);
        entity.Id = 42;

        TrainingSessionDetailDto detail = entity.ToDetailDto();

        Assert.Equal(42, detail.Id);
        Assert.Equal(entity.SessionId, detail.SessionId);
        Assert.Equal("gowning-v1", detail.ProcedureId);
        Assert.Equal(75, detail.Score);
        Assert.False(detail.Passed);
        Assert.Equal(1, detail.ErrorCount);
        Assert.Single(detail.Steps);
        Assert.Single(detail.Errors);
        Assert.Equal("Android 12", detail.DeviceOperatingSystem);
    }

    [Fact]
    public async Task Create_NewSessionId_InsertsAndReportsCreated()
    {
        Mock<ITrainingSessionRepository> repository = new Mock<ITrainingSessionRepository>();
        repository.Setup(r => r.GetBySessionIdAsync(It.IsAny<Guid>())).ReturnsAsync((TrainingSession?)null);
        repository.Setup(r => r.CreateAsync(It.IsAny<TrainingSession>()))
            .ReturnsAsync((TrainingSession s) => s);
        TrainingSessionService service =
            new TrainingSessionService(NullLogger<TrainingSessionService>.Instance, repository.Object);

        TrainingSessionCreateResult result = await service.Create(ValidDto(Guid.NewGuid().ToString()));

        Assert.True(result.Created);
        repository.Verify(r => r.CreateAsync(It.IsAny<TrainingSession>()), Times.Once);
    }

    [Fact]
    public async Task Create_ExistingSessionId_ReturnsExistingWithoutInsert()
    {
        Guid sessionId = Guid.NewGuid();
        TrainingSession existing = ValidDto(sessionId.ToString()).ToEntity(DateTime.UtcNow);
        existing.Id = 7;
        Mock<ITrainingSessionRepository> repository = new Mock<ITrainingSessionRepository>();
        repository.Setup(r => r.GetBySessionIdAsync(sessionId)).ReturnsAsync(existing);
        TrainingSessionService service =
            new TrainingSessionService(NullLogger<TrainingSessionService>.Instance, repository.Object);

        TrainingSessionCreateResult result = await service.Create(ValidDto(sessionId.ToString()));

        Assert.False(result.Created);
        Assert.Equal(7, result.Session.Id);
        repository.Verify(r => r.CreateAsync(It.IsAny<TrainingSession>()), Times.Never);
    }
}
