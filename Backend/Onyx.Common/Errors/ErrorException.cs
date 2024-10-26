using FluentResults;

namespace Onyx.Common.Errors;

/// <summary>
/// An exception type that is used then a failed result type is needed to be thrown
/// </summary>
/// <param name="result">The failed result to throw.</param>
public class ErrorException<_>(Result<_> result) : Exception
{
    public List<IError> Errors { get; set; } = result.Errors;
}