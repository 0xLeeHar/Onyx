using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Onyx.Api.Options;
using Onyx.Common.Errors;

namespace Onyx.Api.Extensions;

public static class AuthenticationBuilderExtensions
{
    /// <summary>
    /// Adds the API Key auth handler to the auth builder.
    /// </summary>
    /// <returns></returns>
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, Action<ApiKeyAuthenticationOptions> options)
    {
        return builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, options);
    }
}

/// <summary>
/// OAuth Scheme options for the API Key auth handler
/// </summary>
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "APIKEY";

    public string Scheme => DefaultScheme;
    public string AuthenticationType => DefaultScheme;
}

/// <summary>
/// Simple API Key authorisation handler.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IOptions<AuthOptions> _authOptions;
    private const string ProblemDetailsContentType = "application/problem+json";
    private const string ApiKeyHeaderName = "api-key";
    
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock,
        IOptions<AuthOptions> authOptions) 
        : base(options, logger, encoder, clock)
    {
        _authOptions = authOptions;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var apiKeyValue = apiKeyHeaderValues.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(apiKeyValue))
        {
            return AuthenticateResult.NoResult();
        }

        if (_authOptions.Value.ApiKey == apiKeyValue)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, "8b0136fd-5549-4197-90de-7e2db159e0c2"),
                new (ClaimTypes.Name, "SYSTEM")
            };
            
            var ticket = new AuthenticationTicket(
                new ClaimsPrincipal(
                    new List<ClaimsIdentity>
                    {
                        new ClaimsIdentity(claims, Options.AuthenticationType)
                    }), 
                Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }

        await Task.CompletedTask;
        
        return AuthenticateResult.Fail("Invalid API Key");
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var problemDetails = new UnauthorizedProblemDetails("HTTP header 'api-key' was not provided");
        
        Response.StatusCode = 401;
        Response.ContentType = ProblemDetailsContentType;
        
        await Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}