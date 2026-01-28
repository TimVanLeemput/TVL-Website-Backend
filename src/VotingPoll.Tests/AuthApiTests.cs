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
using VotingPoll.Infrastructure.Data;

public class AuthApiTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _output;

    public AuthApiTests(ITestOutputHelper output)
    {
        _output = output;
        _factory = new AuthTestFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    private async Task<AppDbContext> GetFreshAppDbContextScope()
    {
        IServiceScope scope = _factory.Services.CreateScope();
        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        return db;
    }

    private StringContent ToJson(object obj) =>
        new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    private async Task<HttpResponseMessage> RegisterUser(string email, string password) =>
        await _client.PostAsync("/api/auth/register", ToJson(new { email, password }), TestContext.Current.CancellationToken);


    [Fact]
    public async Task Register_Returns200_WithValidCredentials()
    {
        await GetFreshAppDbContextScope();

        HttpResponseMessage response = await RegisterUser("test@example.com", "Password1!");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_Returns409_WhenEmailAlreadyExists()
    {
        await GetFreshAppDbContextScope();

        await RegisterUser("duplicate@example.com", "Password1!");
        HttpResponseMessage response = await RegisterUser("duplicate@example.com", "Password1!");

        string body = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Body: {body}");
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(WeakPasswordTestCases))]
    public async Task Register_Returns400_WithWeakPassword(string password)
    {
        await GetFreshAppDbContextScope();

        HttpResponseMessage response = await RegisterUser("test@example.com", password);

        string body = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Password: '{password}' -> {response.StatusCode} | {body}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_Returns403_WhenEmailNotVerified()
    {
        await GetFreshAppDbContextScope();

        await RegisterUser("unverified@example.com", "Password1!");
        HttpResponseMessage response = await _client.PostAsync("/api/auth/login",
            ToJson(new { email = "unverified@example.com", password = "Password1!" }),
            TestContext.Current.CancellationToken);

        string body = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Body: {body}");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Login_Returns401_WithWrongPassword()
    {
        await GetFreshAppDbContextScope();

        HttpResponseMessage response = await _client.PostAsync("/api/auth/login",
            ToJson(new { email = "nobody@example.com", password = "WrongPassword1!" }),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_Returns200_RegardlessOfEmail()
    {
        await GetFreshAppDbContextScope();

        HttpResponseMessage response = await _client.PostAsync("/api/auth/forgot-password",
            ToJson(new { email = "doesnotexist@example.com" }),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_Returns400_WithInvalidToken()
    {
        await GetFreshAppDbContextScope();

        HttpResponseMessage response = await _client.PostAsync("/api/auth/reset-password",
            ToJson(new { token = "invalid-token-xyz", newPassword = "NewPassword1!" }),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    #region Member Data

    public static IEnumerable<object[]> WeakPasswordTestCases()
    {
        yield return new object[] { "short1!" };        // too short
        yield return new object[] { "nouppercase1!" };  // no uppercase letter
        yield return new object[] { "NoDigitsHere!" };  // no digit
        yield return new object[] { "NoSpecialChar1" }; // no special character
        yield return new object[] { "" };               // empty
    }

    #endregion

    private class AuthTestFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<AppDbContext>();
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("AuthTestDb"));
            });
        }
    }
}
