using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VotingPoll.Core.Exceptions;

namespace VotingPoll.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // waits for the whole pipeline to complete
        }
        catch (PollNotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                pollId = ex.PollId
            });
        }
        catch (PollClosedException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                pollId = ex.PollId
            });
        }
        catch (AlreadyVotedException ex)
        {
            context.Response.StatusCode = 409;
            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                userId = ex.UserId,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            // context.Response.StatusCode = 500;
            // context.Response.ContentType = "application/json";

            // ErrorResponse error = new ErrorResponse
            // {
            //     Message = "An unexpected error occurred - TVL",
            //     TraceId = context.TraceIdentifier
            // };
            await ExceptionWriter.WriteErrorResponse(context, 500, "Unhandled exception - Please contact the developer.");

            // await context.Response.WriteAsJsonAsync(error);
        }
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
}