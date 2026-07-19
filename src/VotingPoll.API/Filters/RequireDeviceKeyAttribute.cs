using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VotingPoll.API.Filters;

/// <summary>
/// Authenticates headset uploads via a static device API key in the X-Device-Key header
/// (config: TrainingApi:DeviceKey — Key Vault/env in prod, appsettings.Development
/// locally). Fails closed: no configured key means no request passes. Constant-time
/// comparison; the dashboard's JWT pipeline is untouched by this filter.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireDeviceKeyAttribute : Attribute, IAuthorizationFilter
{
    public const string HeaderName = "X-Device-Key";
    public const string ConfigKey = "TrainingApi:DeviceKey";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        IConfiguration configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        string? configuredKey = configuration[ConfigKey];
        string? providedKey = context.HttpContext.Request.Headers[HeaderName].FirstOrDefault();

        if (string.IsNullOrEmpty(configuredKey) || string.IsNullOrEmpty(providedKey) ||
            !CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(configuredKey),
                Encoding.UTF8.GetBytes(providedKey)))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Missing or invalid device key." });
        }
    }
}
