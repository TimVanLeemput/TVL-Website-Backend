using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VotingPoll.Core.Exceptions;
using VotingPoll.Core.Exceptions.AuthExceptions;

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

        #region PollExceptions

        catch (PollNotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                pollId = ex.PollId
            });
        }
        catch (InvalidPollOptionException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new
                {
                    error = ex.Message,
                    pollOptionId = ex.PollOptionId
                }
            );
        }
        catch (PollOptionNotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new
                {
                    error = ex.Message,
                    pollOptionId = ex.PollOptionId
                }
            );
        }
        catch (ListOfPollOptionsNotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new
                {
                    error = ex.Message,
                    pollId = ex.PollId
                }
            );
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
        catch (VoteNotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new
                {
                    error = ex.Message,
                    voteId = ex.VoteId
                }
            );
        }

        #endregion

        #region AuthExceptions

        catch (EmailAlreadyExistsException ex)
        {
            context.Response.StatusCode = 409;
            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                email = ex.Email
            });
        }
        catch (InvalidCredentialsException ex)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (EmailNotVerifiedException ex)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (InvalidVerificationTokenException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (VerificationEmailCooldownException ex)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (InvalidPasswordResetTokenException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }

        #endregion

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
            // await ExceptionWriter.WriteErrorResponse(context, 500, $"{ex.GetType().Name}:Unhandled exception! Contact developer. {ex.Message}");
            await ExceptionWriter.WriteErrorResponse(context, 500,
                "Unhandled exception - Please contact the developer.");

            // await context.Response.WriteAsJsonAsync(error);
        }
    }
}