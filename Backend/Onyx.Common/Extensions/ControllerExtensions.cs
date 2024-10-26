using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Onyx.Common.Extensions;

public static class ControllerExtensions
{
    /// <summary>
    /// Converts a fluent Result to an ActionResult HTTP response.
    /// </summary>
    /// <param name="controller">The controller to extent</param>
    /// <param name="result">The source result to convert.</param>
    /// <typeparam name="T">The type of the Result</typeparam>
    /// <returns>An action result matching the variant of the result.</returns>
    public static ActionResult<T> ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        // TODO: For now just return the error.
        // Note: In prod code you would hand to handel more cases.
        return controller.StatusCode(500, result.Errors);
    }
}