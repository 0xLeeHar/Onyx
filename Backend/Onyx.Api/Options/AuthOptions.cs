namespace Onyx.Api.Options;

/// <summary>
/// Authentication options for the app configuration
/// </summary>
public sealed class AuthOptions
{
    public static string SectionName => "Auth";

    public string? ApiKey { get; set; }
    public string? Authority { get; set; }
    public string? ClientId { get; set; }
}