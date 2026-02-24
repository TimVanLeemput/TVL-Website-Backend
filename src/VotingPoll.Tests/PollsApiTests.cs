using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Models.DTOs;
using VotingPoll.Infrastructure.Data;

public class PollsApiTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _output;

    public PollsApiTests(ITestOutputHelper output)
    {
        _output = output;
        _factory = new TestFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();


    private async Task<AppDbContext> GetFreshAppDbContextScope()
    {
        IServiceScope _scope = _factory.Services.CreateScope();
        AppDbContext _db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await _db.Database.EnsureDeletedAsync(); // wipe it
        await _db.Database.EnsureCreatedAsync(); // recreate schema
        return _db;
    }

    private static async Task AddPollAndSave(AppDbContext dbContext)
    {
        await dbContext.Polls.AddAsync(new Poll { Title = "Test Poll" });
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetPolls_Returns200()
    {
        AppDbContext _dbContext = await GetFreshAppDbContextScope();


        HttpResponseMessage response = await _client.GetAsync("/api/polls");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetPolls_ReturnsPolls_WhenSeeded()
    {
        AppDbContext dbContext = await GetFreshAppDbContextScope();
        await AddPollAndSave(dbContext);

        HttpResponseMessage response = await _client.GetAsync("/api/polls");
        string content = await response.Content.ReadAsStringAsync();
        PagedResponse<PollDto>? result =
            JsonSerializer.Deserialize<PagedResponse<PollDto>>(
                content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        string pollTitle = result.Items.Where(p => p.Title == "Test Poll").FirstOrDefault().Title;
        _output.WriteLine("Custom output : Poll returned: " + pollTitle);
        Assert.NotNull(result);
        Assert.Contains(result.Items, poll => poll.Title == "Test Poll");
    }

    [Theory]
    [InlineData(999)]
    public async Task GetPoll999_ReturnsNotFound(int pollId)
    {
        AppDbContext dbContext = await GetFreshAppDbContextScope();
        HttpResponseMessage response = await _client.GetAsync($"/api/polls/{pollId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Theory]
    [MemberData(nameof(CreatePollTestCases))]
    public async Task PostPoll_ReturnsExpectedStatusCode(CreatePollDto invalidPoll, HttpStatusCode expectedStatusCode)
    {
        AppDbContext _dbContext = await GetFreshAppDbContextScope();
        // Replaced with MemberData
        // List<CreatePollOptionDto> pollOptions = new List<CreatePollOptionDto>
        // {
        //     new CreatePollOptionDto
        //     {
        //         PollOptionName = "Option AA",
        //     },
        //     // missing second option
        // };
        //
        // CreatePollDto poll = new CreatePollDto
        // {
        //     Title = "Test Poll",
        //     // missing allPollOptions
        // };

        StringContent content =
            new StringContent(JsonSerializer.Serialize(invalidPoll), Encoding.UTF8, "application/json");

        HttpResponseMessage response =
            await _client.PostAsync("/api/polls", content, TestContext.Current.CancellationToken);

        string responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(
            $"Status expected 400 --  Actual Status => {response.StatusCode.GetHashCode()} + {response.StatusCode}");
        _output.WriteLine($"Body: {responseBody}");
        Assert.Equal(expectedStatusCode, response.StatusCode);
    }


    // [Fact]
    // public async Task GetPolls_ReturnsEmptyList_WhenNoPolls()
    // {
    //     // Act
    //     HttpResponseMessage response = await _client.GetAsync("/api/polls");
    //
    //     // Assert
    //     response.EnsureSuccessStatusCode();
    //     string content = await response.Content.ReadAsStringAsync();
    //     List<PollDto>? polls = JsonSerializer.Deserialize<List<PollDto>>(content,
    //         new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    //     Assert.NotNull(polls);
    //     Assert.Empty(polls);
    // }
    //
    // [Fact]
    // public async Task CreatePoll_ReturnsCreatedPoll()
    // {
    //     // Arrange
    //     List<CreatePollOptionDto> createPollOptionDtos = new List<CreatePollOptionDto>
    //     {
    //         new CreatePollOptionDto { PollOptionName = "Option A" },
    //         new CreatePollOptionDto { PollOptionName = "Option B" }
    //     };
    //
    //     CreatePollDto request = new CreatePollDto
    //     {
    //         Title = "Test Poll",
    //         AllPollOptions = createPollOptionDtos
    //     };
    //
    //     StringContent content = new StringContent(
    //         JsonSerializer.Serialize(request),
    //         Encoding.UTF8,
    //         "application/json");
    //
    //     // Act
    //     HttpResponseMessage response = await _client.PostAsync("/api/polls", content);
    //
    //     // Assert
    //     Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    // }

    // [Fact]
    // public async Task Vote_Twice_Returns409()
    // {
    //     // Create a poll first, then vote twice — second vote should return 409
    //     // ... implementation
    // }
    private record PagedResponse<T>(
        List<T> Items,
        int TotalCount,
        int? CurrentPage,
        int?
            PageSize,
        int TotalPages);

    private class TestFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<AppDbContext>();
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        }
    }

    #region POST Member Data

    public static IEnumerable<object[]> CreatePollTestCases()
    {
        #region Failure Cases

        // Only one poll option - not enough options
        yield return new object[]
        {
            new CreatePollDto
            {
                Title = "Test Poll",
                AllPollOptions = new List<CreatePollOptionDto>
                {
                    new CreatePollOptionDto { PollOptionName = "Option AA" }
                }
            },
            HttpStatusCode.BadRequest
        };

        // Missing options entirely
        yield return new object[]
        {
            new CreatePollDto
            {
                Title = "Test Poll",
                AllPollOptions = null
            },
            HttpStatusCode.BadRequest
        };

        // Missing title entirely
        yield return new object[]
        {
            new CreatePollDto
            {
                Title = null,
                AllPollOptions = new List<CreatePollOptionDto>
                {
                    new CreatePollOptionDto { PollOptionName = "Option AA" },
                    new CreatePollOptionDto { PollOptionName = "Option BB" }
                }
            },
            HttpStatusCode.BadRequest
        };

        // Title not meeting length requirement - too short
        yield return new object[]
        {
            new CreatePollDto
            {
                Title = "Ti",
                AllPollOptions = new List<CreatePollOptionDto>
                {
                    new CreatePollOptionDto { PollOptionName = "Option AA" },
                    new CreatePollOptionDto { PollOptionName = "Option BB" }
                }
            },
            HttpStatusCode.BadRequest
        };

        // Title is empty
        yield return new object[]
        {
            new CreatePollDto
            {
                Title = string.Empty,
                AllPollOptions = new List<CreatePollOptionDto>
                {
                    new CreatePollOptionDto { PollOptionName = "Option AA" },
                    new CreatePollOptionDto { PollOptionName = "Option BB" }
                }
            },
            HttpStatusCode.BadRequest
        };

        // Title not meeting length requirement - too long
        yield return new object[]
        {
            new CreatePollDto
            {
                Title = new string('A', 101),
                AllPollOptions = new List<CreatePollOptionDto>
                {
                    new CreatePollOptionDto { PollOptionName = "Option AA" },
                    new CreatePollOptionDto { PollOptionName = "Option BB" }
                }
            },
            HttpStatusCode.BadRequest
        };

        // PollOptionName not meeting requirements - empty string
        yield return new object[]
        {
            new CreatePollDto
            {
                Title = "Test Poll",
                AllPollOptions = new List<CreatePollOptionDto>
                {
                    new CreatePollOptionDto { PollOptionName = "" },
                    new CreatePollOptionDto { PollOptionName = "Option BB" }
                }
            },
            HttpStatusCode.BadRequest
        };

        // PollOptionName not meeting requirements - null string
        yield return new object[]
        {
            new CreatePollDto
            {
                Title = "Test Poll",
                AllPollOptions = new List<CreatePollOptionDto>
                {
                    new CreatePollOptionDto { PollOptionName = null },
                    new CreatePollOptionDto { PollOptionName = "Option BB" }
                }
            },
            HttpStatusCode.BadRequest
        };

        // Above upper boundary limit for AllPollOptions (11 options)
        yield return new object[]
        {
            new CreatePollDto
            {
                Title = "Test Poll",
                AllPollOptions = Enumerable.Range(1, 11)
                    .Select(i => new CreatePollOptionDto { PollOptionName = $"Option {i}" })
                    .ToList()
            },
            HttpStatusCode.BadRequest
        };

        #endregion

        #region Success Cases

        yield return new object[]
        {
            new CreatePollDto
            {
                Title = "Test Poll",
                AllPollOptions = new List<CreatePollOptionDto>
                {
                    new CreatePollOptionDto { PollOptionName = "Option AA" },
                    new CreatePollOptionDto { PollOptionName = "Option BB" }
                }
            },
            HttpStatusCode.Created
        };

        #endregion
    }

    #endregion
}