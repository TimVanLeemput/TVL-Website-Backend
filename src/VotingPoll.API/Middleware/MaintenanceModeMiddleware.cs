using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace VotingPoll.API.Middleware;

public class MaintenanceModeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MaintenanceModeMiddleware> _logger;

    public MaintenanceModeMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<MaintenanceModeMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        bool enforceJsonWrites = _configuration.GetValue("BackendSafeguards:EnforceJsonWriteContentType", true);
        if (enforceJsonWrites && IsWriteMethod(context.Request.Method) && !context.Request.HasJsonContentType())
        {
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Only application/json is accepted for write operations."
            });
            return;
        }

        await _next(context);
    }

    private static bool IsWriteMethod(string method)
    {
        return HttpMethods.IsPost(method)
               || HttpMethods.IsPut(method)
               || HttpMethods.IsPatch(method)
               || HttpMethods.IsDelete(method);
    }
}