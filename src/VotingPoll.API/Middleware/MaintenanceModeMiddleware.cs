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
        bool lockdownEnabled = _configuration.GetValue("BackendSafeguards:Lockdown:Enabled", true);
        bool allowLocalhost = _configuration.GetValue("BackendSafeguards:Lockdown:AllowLocalhost", true);
        bool enforceJsonWrites = _configuration.GetValue("BackendSafeguards:EnforceJsonWriteContentType", true);
        string bypassHeaderName = _configuration.GetValue<string>("BackendSafeguards:Lockdown:BypassHeaderName")
                                  ?? "X-Backend-Bypass";
        string bypassHeaderValue = _configuration.GetValue<string>("BackendSafeguards:Lockdown:BypassHeaderValue")
                                   ?? string.Empty;

        if (enforceJsonWrites && IsWriteMethod(context.Request.Method) && !context.Request.HasJsonContentType())
        {
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Only application/json is accepted for write operations."
            });
            return;
        }

        if (!lockdownEnabled)
        {
            await _next(context);
            return;
        }

        bool isLocalRequest = IsLocalRequest(context);
        bool hasBypassHeader = !string.IsNullOrWhiteSpace(bypassHeaderValue)
                               && context.Request.Headers.TryGetValue(bypassHeaderName, out var incomingValue)
                               && string.Equals(incomingValue.ToString(), bypassHeaderValue, StringComparison.Ordinal);

        if ((allowLocalhost && isLocalRequest) || hasBypassHeader)
        {
            await _next(context);
            return;
        }

        _logger.LogWarning(
            "Blocked request while backend lockdown is enabled. Method: {Method}, Path: {Path}, RemoteIp: {RemoteIp}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown");

        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Backend is temporarily locked down."
        });
    }

    private static bool IsWriteMethod(string method)
    {
        return HttpMethods.IsPost(method)
               || HttpMethods.IsPut(method)
               || HttpMethods.IsPatch(method)
               || HttpMethods.IsDelete(method);
    }

    private static bool IsLocalRequest(HttpContext context)
    {
        IPAddress? remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp is null) return false;

        return IPAddress.IsLoopback(remoteIp) ||
               (remoteIp.IsIPv4MappedToIPv6 && IPAddress.IsLoopback(remoteIp.MapToIPv4()));
    }
}
