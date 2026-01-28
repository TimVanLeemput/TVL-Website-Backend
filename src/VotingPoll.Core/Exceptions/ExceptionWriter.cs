using Microsoft.AspNetCore.Http;

namespace VotingPoll.Core.Exceptions;

public static class ExceptionWriter
{
    public static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            Message = message,
            StatusCode = statusCode,
            TraceId = context.TraceIdentifier
        });
    }

    private class ErrorResponse()
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string TraceId { get; set; }
    }
}