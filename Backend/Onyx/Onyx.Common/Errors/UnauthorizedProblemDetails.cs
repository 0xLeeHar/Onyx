using Microsoft.AspNetCore.Mvc;

namespace Onyx.Common.Errors;

public class UnauthorizedProblemDetails : ProblemDetails
{
    public UnauthorizedProblemDetails(string? details = null)
    {
        Title = "Call not authorized.";
        Detail = details;
        Status = 401;
    }
}