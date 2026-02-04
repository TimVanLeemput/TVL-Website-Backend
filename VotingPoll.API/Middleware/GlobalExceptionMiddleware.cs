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
            _logger.LogInformation("Maintenance Mode is not Active - Entering Global Exception Middleware");
            await _next(context); // waits for the whole pipeline to complete
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception - TVL");

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            ErrorResponse error = new ErrorResponse
            {
                Message = "An unexpected error occurred - TVL",
                TraceId = context.TraceIdentifier
            };

            await context.Response.WriteAsJsonAsync(error);
        }
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
}