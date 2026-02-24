//  The order is strict:
//1. Register services     (builder.Services.Add...) -> What the app will need and use
//2. Build the app         (builder.Build()) -> Builds the app
// -----------------------------------------------------------------------------------------------------------------
//3. Configure pipeline    (app.Use..., app.Map...)
//4. Run                   (app.Run())

using System.Text;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VotingPoll.API.Controllers.Authentication;
using VotingPoll.API.Middleware;
using VotingPoll.Core.Interfaces.Authentication;
using VotingPoll.Core.Interfaces.Repositories;
using VotingPoll.Core.Interfaces.ServicesInterfaces;
using VotingPoll.Core.Interfaces.ServicesInterfaces.Authentication;
using VotingPoll.Core.Services;
using VotingPoll.Core.Services.Authentication;
using VotingPoll.Core.Services.Authentication.Token;
using VotingPoll.Infrastructure.Data;
using VotingPoll.Infrastructure.Repositories;
using VotingPoll.Infrastructure.Validation;

// --------------------------------------------------------APP CONTAINER / SETUP--------------------------------------

#region App container

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

#region Repositories

builder.Services.AddScoped<IPollRepository, PollRepository>();
builder.Services.AddScoped<IPollOptionRepository, PollOptionRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();

#endregion

#region Custom Services

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

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

#region Azure

string keyVaultUri = builder.Configuration["KeyVaultUrl"];
SecretClient secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
#endregion

#endregion

#endregion

WebApplication app = builder.Build();
// ----------------------------------------------CONTAINER IS SEALED AFTER THIS POINT-------------------------------
// -------------------------------------------------APP IS RUNNING AFTER THIS POINT---------------------------------

#region Middleware

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication(); // Validates JWT token
app.UseAuthorization(); // Checks if a User has access to the endpoint ([Authorize])

app.MapControllers();

#endregion

app.Run();