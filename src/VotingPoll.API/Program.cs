//  The order is strict:
//1. Register services     (builder.Services.Add...) -> What the app will need and use
//2. Build the app         (builder.Build()) -> Builds the app
// -----------------------------------------------------------------------------------------------------------------
//3. Configure pipeline    (app.Use..., app.Map...)
//4. Run                   (app.Run())

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VotingPoll.API.Middleware;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Services;
using VotingPoll.Infrastructure.Data;
using VotingPoll.Infrastructure.Repositories;
using VotingPoll.Infrastructure.Validation;

// --------------------------------------------------------APP CONTAINER / SETUP--------------------------------------

#region App container

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

#region Repositories

builder.Services.AddScoped<IPollRepository, PollRepository>();
builder.Services.AddScoped<IPollOptionRepository, PollOptionRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();

#endregion

#region Custom Services

builder.Services.AddScoped<IVotingService, VotingService>();

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

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

#endregion

app.Run();