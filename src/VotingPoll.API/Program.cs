//  The order is strict:
//1. Register services     (builder.Services.Add...) -> What the app will need and use
//2. Build the app         (builder.Build()) -> Builds the app
// -----------------------------------------------------------------------------------------------------------------
//3. Configure pipeline    (app.Use..., app.Map...)
//4. Run                   (app.Run())


// --------------------------------------------------------APP CONTAINER / SETUP--------------------------------------

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VotingPoll.API.Middleware;
using VotingPoll.Core.Entities;
using VotingPoll.Infrastructure.Data;
using VotingPoll.Infrastructure.Repositories;
using VotingPoll.Infrastructure.Validation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPollRepository, PollRepository>();
builder.Services.AddScoped<IPollOptionRepository, PollOptionRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddControllers();
// builder.Services.AddScoped<IValidator<Poll>, CreatePollDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreatePollDtoValidator>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration
            .GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("VotingPoll.API"))
);

WebApplication app = builder.Build();
// ----------------------------------------------CONTAINER IS SEALED AFTER THIS POINT-------------------------------
// -------------------------------------------------APP IS RUNNING AFTER THIS POINT---------------------------------
// app.UseMiddleware<MaintenanceModeMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

// app.UseHttpsRedirection();


app.Run();