//  The order is strict:
//1. Register services     (builder.Services.Add...) -> What the app will need and use
//2. Build the app         (builder.Build()) -> Builds the app
// -----------------------------------------------------------------------------------------------------------------
//3. Configure pipeline    (app.Use..., app.Map...)
//4. Run                   (app.Run())

using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using VotingPoll.API.Middleware;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Services;
using VotingPoll.Infrastructure.Data;
using VotingPoll.Infrastructure.Repositories;
using VotingPoll.Infrastructure.Validation;

// --------------------------------------------------------APP CONTAINER / SETUP--------------------------------------
// PROTECTING BACKEND WHILE IN DEV MODE
// Quick disable later (when you want backend open again)
//
// 1. Set BackendSafeguards:Lockdown:Enabled to false.
// 2. Optionally set BackendSafeguards:EnforceJsonWriteContentType to false.
// 3. Remove/comment app.UseRateLimiter(); and app.UseMiddleware<MaintenanceModeMiddleware>(); in
// TVL-Website-Backend/src/VotingPoll.API/Program.cs:97.
#region App container

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2 * 1024 * 1024; // 2 MB
    options.MultipartHeadersCountLimit = 20;
    options.MultipartHeadersLengthLimit = 4096;
    options.MultipartBoundaryLengthLimit = 128;
    options.ValueCountLimit = 64;
    options.ValueLengthLimit = 4096;
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 2 * 1024 * 1024; // 2 MB
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            })
    );
});

#region Repositories

builder.Services.AddScoped<IPollRepository, PollRepository>();
builder.Services.AddScoped<IPollOptionRepository, PollOptionRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();

#endregion

#region Custom Services

builder.Services.AddScoped<IVotingService, VotingService>();
builder.Services.AddScoped<IPollService, PollService>();
builder.Services.AddScoped<IPollOptionService, PollOptionService>();

#endregion

#region Validators

builder.Services.AddValidatorsFromAssemblyContaining<CreatePollDtoValidator>();

#endregion

#region Database Connection

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration
            .GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("VotingPoll.API"))
);

#endregion

#endregion

WebApplication app = builder.Build();
// ----------------------------------------------CONTAINER IS SEALED AFTER THIS POINT-------------------------------
// -------------------------------------------------APP IS RUNNING AFTER THIS POINT---------------------------------

#region Middleware

app.UseRateLimiter();
app.UseMiddleware<MaintenanceModeMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

#endregion

app.Run();
