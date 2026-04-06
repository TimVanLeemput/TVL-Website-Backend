//  The order is strict:
//1. Register services     (builder.Services.Add...) -> What the app will need and use
//2. Build the app         (builder.Build()) -> Builds the app
// -----------------------------------------------------------------------------------------------------------------
//3. Configure pipeline    (app.Use..., app.Map...)
//4. Run                   (app.Run())

using System.Text;
using System.Threading.RateLimiting;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Resend;
using VotingPoll.API.Middleware;
using VotingPoll.Core.Interfaces.Authentication;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Core.Interfaces.Repositories.Authentication;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Interfaces.ServicesInterfaces.Authentication;
using VotingPoll.Core.Services;
using VotingPoll.Core.Services.Authentication;
using VotingPoll.Core.Services.Authentication.Token;
using VotingPoll.Core.Services.EmailService;
using VotingPoll.Infrastructure.Data;
using VotingPoll.Infrastructure.Repositories;
using VotingPoll.Infrastructure.Repositories.Authentication;
using VotingPoll.Infrastructure.Validation;

// --------------------------------------------------------APP CONTAINER / SETUP--------------------------------------

#region App container

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

#region API Endpoints

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Voting Poll API",
        Version = "v1",
        Description = "The API for the Voting Poll application",
    });
});

#endregion

#region Repositories

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<IPollRepository, PollRepository>();
builder.Services.AddScoped<IPollOptionRepository, PollOptionRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();

#endregion

#region Custom Services

//
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IVotingService, VotingService>();
builder.Services.AddScoped<IPollService, PollService>();
builder.Services.AddScoped<IPollOptionService, PollOptionService>();

#endregion

#region Validators

builder.Services.AddValidatorsFromAssemblyContaining<CreatePollDtoValidator>();

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://timvanleemput.com",
                "http://localhost:5000" /*local backend app*/,
                "http://localhost:5002" /*Local front end app*/)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

#endregion

#region Azure

string keyVaultUri = (builder.Configuration["KeyVaultUrl"] ?? "").Trim();
if (!string.IsNullOrWhiteSpace(keyVaultUri) && Uri.IsWellFormedUriString(keyVaultUri, UriKind.Absolute))
{
    SecretClient secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
    builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
}

#endregion

#region Database Connection

// DatabaseProvider selects which connection string to use.
// Production (appsettings.json):            "DefaultConnection" -> Key Vault secret ConnectionStrings--DefaultConnection (Neon)
// Local dev (appsettings.Development.json): "PostgreSQL-Local"  -> localhost, never stored in Key Vault
string databaseProvider = builder.Configuration["DatabaseProvider"]!;
builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(databaseProvider),
            b => b.MigrationsAssembly("VotingPoll.Infrastructure"));
    }
);

#endregion

#region Authentication

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // enforce the iss claim
            ValidateAudience = true, // enforce the aud claim
            ValidateLifetime = true, // enforce exp claim (rejects expired tokens)
            ValidateIssuerSigningKey = true, // enforce signature validation

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });
builder.Services.AddAuthorization();

#endregion

#region Resend

builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(options =>
{
    options.ApiToken = builder.Configuration["Resend:ApiKey"];
});
builder.Services.AddTransient<IResend, ResendClient>();

builder.Services.AddTransient<EmailService>();

#endregion

#region Health Checks

builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>("DB Health Check");

#endregion

#region Security

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ??
                          httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 40,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});
//idniwjqodqodnoqndononononijfijfefejfeijf ei dont think this sound ery
builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = 10_240; });

#endregion

#endregion

WebApplication app = builder.Build();
// ----------------------------------------------CONTAINER IS SEALED AFTER THIS POINT-------------------------------
// -------------------------------------------------APP IS RUNNING AFTER THIS POINT---------------------------------

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication(); // Validates JWT token
app.UseAuthorization(); // Checks if a User has access to the endpoint ([Authorize])

app.MapControllers();

app.MapHealthChecks("/health");

app.UseRateLimiter();

#endregion

app.Run();