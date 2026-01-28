using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace VotingPoll.API.Middleware;
using System.Diagnostics;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("--> {Method} {Path}", context.Request.Method, context.Request.Path);

        await _next(context); // waits for the whole pipeline to complete 

        stopwatch.Stop();
        _logger.LogInformation("<-- {Method} {Path} responded {StatusCode} in {Elapsed}ms",
            context.Request.Method, context.Request.Path,
            context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}