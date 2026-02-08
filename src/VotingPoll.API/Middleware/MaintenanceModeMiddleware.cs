using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VotingPoll.API.Middleware;

public class MaintenanceModeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MaintenanceModeMiddleware> _logger;

    public MaintenanceModeMiddleware(RequestDelegate next,
        IConfiguration configuration, ILogger<MaintenanceModeMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // if(_configuration.GetValue<bool>("MaintenanceMode"))
        //     context.Response.StatusCode = 503;
        // else await _next(context);
        // bool isAnyMaintenanceModeActive = _configuration.GetSection("MaintenanceMode").GetChildren()
        //     .Any(x => bool.TryParse(x.Value, out var maintenanceModeIsActive)
        //               && maintenanceModeIsActive == true);
        
        
        // var section = _configuration.GetSection("MaintenanceMode").Get<MaintenanceModeOptions>();
        // bool isAnyModesTrue = section?.Modes != null && (section.Modes.Any(x => x));
        // if (isAnyModesTrue)
        // {
        //     _logger.LogInformation($"Maintenance Mode is Active : {isAnyModesTrue}");
        //     context.Response.StatusCode = 503;
        //     return;
        // }

        await _next(context);  //It's not just "call the next middleware" - it's "call the
        // next middleware and wait for it to fully complete, including the response."

        // Check if any of the maintenance modes are true - if so, return 503
        // An other syntax for the TryParse: 
        //.Any(x =>
        // {
        //     bool canParse = bool.TryParse(x.Value, out bool maintenanceModeIsActive);
        //
        //     if (canParse && maintenanceModeIsActive)
        //         return true;
        //
        //     return false;
        // })
        // _logger.LogInformation("Maintenance Mode: {isAnyMaintenanceModeActive}", isAnyMaintenanceModeActive);

        // if (isAnyMaintenanceModeActive) context.Response.StatusCode = 503;
    }
}