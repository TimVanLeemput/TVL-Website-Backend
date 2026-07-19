using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VotingPoll.Core.Entities.Training;
using VotingPoll.Core.Models.DTOs;
using VotingPoll.Infrastructure.Data;

public class TrainingSessionsApiTests : IDisposable
{
    private const string DeviceKey = "test-device-key";

    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public TrainingSessionsApiTests()
    {
        _factory = new TestFactory(addTestAuth: true);
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    private async Task<AppDbContext> GetFreshAppDbContextScope()
    {
        IServiceScope _scope = _factory.Services.CreateScope();
        AppDbContext _db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();
        return _db;
    }

    private static CreateTrainingSessionDto ValidSession(string? sessionId = null)
    {
        return new CreateTrainingSessionDto
        {
            SchemaVersion = 1,
            SessionId = sessionId ?? Guid.NewGuid().ToString(),
            ProcedureId = "gowning-v1",
            ProcedureTitle = "Cleanroom Gowning (Aseptic Entry)",
            StartedAtUtc = "2026-07-19T12:00:00Z",
            CompletedAtUtc = "2026-07-19T12:02:03Z",
            DurationSeconds = 123.4,
            Score = 75,
            Passed = false,
            PassingScore = 80,
            Steps = new List<CreateTrainingStepDto>
            {
                new CreateTrainingStepDto
                    { Index = 0, Instruction = "Sanitize your hands", DurationSeconds = 5.2, Completed = true },
                new CreateTrainingStepDto
                    { Index = 1, Instruction = "Put on the hairnet", DurationSeconds = 8.0, Completed = true },
            },
            Errors = new List<CreateTrainingErrorDto>
            {
                new CreateTrainingErrorDto
                {
                    StepIndex = 1, RuleType = "ContaminationTouchRule", Severity = "Critical",
                    ScoreDeduction = 25, AtSeconds = 30.1
                },
            },
            Device = new CreateTrainingDeviceDto
                { Model = "Quest 3", OperatingSystem = "Android 12", AppVersion = "1.0" },
        };
    }

    private HttpRequestMessage PostRequest(CreateTrainingSessionDto dto, string? deviceKey = DeviceKey)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/api/training-sessions")
        {
            Content = JsonContent.Create(dto),
        };
        if (deviceKey != null)
            request.Headers.Add("X-Device-Key", deviceKey);
        return request;
    }

    #region POST

    [Fact]
    public async Task PostSession_WithDeviceKey_Returns201AndPersistsChildren()
    {
        AppDbContext dbContext = await GetFreshAppDbContextScope();

        HttpResponseMessage response = await _client.SendAsync(PostRequest(ValidSession()),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        TrainingSession? stored = await dbContext.TrainingSessions
            .Include(ts => ts.Steps).Include(ts => ts.Errors)
            .FirstOrDefaultAsync(TestContext.Current.CancellationToken);
        Assert.NotNull(stored);
        Assert.Equal("gowning-v1", stored.ProcedureId);
        Assert.Equal(2, stored.Steps.Count);
        Assert.Single(stored.Errors);
        Assert.Equal("ContaminationTouchRule", stored.Errors[0].RuleType);
        Assert.Equal(new DateTime(2026, 7, 19, 12, 0, 0, DateTimeKind.Utc), stored.StartedAtUtc);
    }

    [Fact]
    public async Task PostSession_SameSessionIdTwice_IsIdempotent()
    {
        AppDbContext dbContext = await GetFreshAppDbContextScope();
        string sessionId = Guid.NewGuid().ToString();

        HttpResponseMessage first = await _client.SendAsync(PostRequest(ValidSession(sessionId)),
            TestContext.Current.CancellationToken);
        HttpResponseMessage second = await _client.SendAsync(PostRequest(ValidSession(sessionId)),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Equal(1, await dbContext.TrainingSessions.CountAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task PostSession_MissingOrWrongDeviceKey_Returns401()
    {
        await GetFreshAppDbContextScope();

        HttpResponseMessage missing = await _client.SendAsync(PostRequest(ValidSession(), deviceKey: null),
            TestContext.Current.CancellationToken);
        HttpResponseMessage wrong = await _client.SendAsync(PostRequest(ValidSession(), deviceKey: "wrong-key"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, missing.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, wrong.StatusCode);
    }

    public static IEnumerable<object[]> InvalidSessionCases()
    {
        CreateTrainingSessionDto wrongSchema = ValidSession();
        wrongSchema.SchemaVersion = 2;
        yield return new object[] { wrongSchema };

        CreateTrainingSessionDto badGuid = ValidSession();
        badGuid.SessionId = "not-a-guid";
        yield return new object[] { badGuid };

        CreateTrainingSessionDto completedBeforeStarted = ValidSession();
        completedBeforeStarted.CompletedAtUtc = "2026-07-19T11:00:00Z";
        yield return new object[] { completedBeforeStarted };

        CreateTrainingSessionDto noSteps = ValidSession();
        noSteps.Steps = new List<CreateTrainingStepDto>();
        yield return new object[] { noSteps };

        CreateTrainingSessionDto badTimestamp = ValidSession();
        badTimestamp.StartedAtUtc = "yesterday";
        yield return new object[] { badTimestamp };
    }

    [Theory]
    [MemberData(nameof(InvalidSessionCases))]
    public async Task PostSession_InvalidPayload_Returns400(CreateTrainingSessionDto invalidDto)
    {
        await GetFreshAppDbContextScope();

        HttpResponseMessage response = await _client.SendAsync(PostRequest(invalidDto),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region GET

    [Fact]
    public async Task GetSessions_WithoutJwt_Returns401()
    {
        using WebApplicationFactory<Program> noAuthFactory = new TestFactory(addTestAuth: false);
        using HttpClient noAuthClient = noAuthFactory.CreateClient();

        HttpResponseMessage response = await noAuthClient.GetAsync("/api/training-sessions",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetSessions_FiltersAndPages_NewestFirst()
    {
        await GetFreshAppDbContextScope();

        for (int i = 0; i < 3; i++)
        {
            CreateTrainingSessionDto dto = ValidSession();
            dto.StartedAtUtc = $"2026-07-1{i + 1}T12:00:00Z";
            dto.CompletedAtUtc = $"2026-07-1{i + 1}T12:02:00Z";
            await _client.SendAsync(PostRequest(dto), TestContext.Current.CancellationToken);
        }

        CreateTrainingSessionDto otherProcedure = ValidSession();
        otherProcedure.ProcedureId = "other-procedure";
        await _client.SendAsync(PostRequest(otherProcedure), TestContext.Current.CancellationToken);

        HttpResponseMessage response = await _client.GetAsync(
            "/api/training-sessions?procedureId=gowning-v1&fromUtc=2026-07-12T00:00:00Z&page=1&pageSize=10",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        JsonElement payload = JsonDocument.Parse(json).RootElement;

        Assert.Equal(2, payload.GetProperty("totalCount").GetInt32());
        JsonElement items = payload.GetProperty("items");
        Assert.Equal(2, items.GetArrayLength());
        // Newest first: 13th before 12th; the 11th is excluded by fromUtc.
        Assert.StartsWith("2026-07-13", items[0].GetProperty("startedAtUtc").GetString());
        Assert.StartsWith("2026-07-12", items[1].GetProperty("startedAtUtc").GetString());
    }

    [Fact]
    public async Task GetSessionById_ReturnsDetail_AndUnknownIdReturns404()
    {
        await GetFreshAppDbContextScope();

        HttpResponseMessage created = await _client.SendAsync(PostRequest(ValidSession()),
            TestContext.Current.CancellationToken);
        TrainingSessionSummaryDto? summary =
            await created.Content.ReadFromJsonAsync<TrainingSessionSummaryDto>(JsonOptions,
                TestContext.Current.CancellationToken);
        Assert.NotNull(summary);

        HttpResponseMessage detailResponse = await _client.GetAsync($"/api/training-sessions/{summary.Id}",
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);

        TrainingSessionDetailDto? detail =
            await detailResponse.Content.ReadFromJsonAsync<TrainingSessionDetailDto>(JsonOptions,
                TestContext.Current.CancellationToken);
        Assert.NotNull(detail);
        Assert.Equal(2, detail.Steps.Count);
        Assert.Single(detail.Errors);
        Assert.Equal(1, detail.ErrorCount);

        HttpResponseMessage notFound = await _client.GetAsync("/api/training-sessions/999999",
            TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, notFound.StatusCode);
    }

    #endregion

    private class TestFactory : WebApplicationFactory<Program>
    {
        private readonly bool _addTestAuth;

        public TestFactory(bool addTestAuth)
        {
            _addTestAuth = addTestAuth;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["TrainingApi:DeviceKey"] = DeviceKey,
                    // The real JwtBearer pipeline runs when test auth is off; without a
                    // secret it 500s instead of returning 401.
                    ["Jwt:Issuer"] = "test-issuer",
                    ["Jwt:Audience"] = "test-audience",
                    ["Jwt:Secret"] = "test-secret-key-with-at-least-32-characters!",
                });
            });

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<AppDbContext>();
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TrainingTestDb"));

                if (_addTestAuth)
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                }
            });
        }
    }

    private class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "admin@test.com"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Test");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            AuthenticationTicket ticket = new AuthenticationTicket(principal, "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
