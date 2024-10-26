namespace Onyx.Common;

/// <summary>
/// An generic HTTP response for asynchronous APIs
/// </summary>
/// <param name="ResourceId">The ID of the resource that is being created</param>
public sealed record AsyncApiResponse(Guid ResourceId);