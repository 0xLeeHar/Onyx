using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Onyx.Common.Extensions;

namespace Onyx.Services.Products.Tests.Unit;

public class ControllerExtensionTests
{
    [Theory]
    [MemberData(nameof(ResultTypeData))]
    public void ToActionResult_WithResult_ReturnsCorrectHttpCode(Result<ResponseObjectFake> resultVariant, int expectedHttpCode)
    {
        // Arrange
        var controller = new ControllerFake();

        // Act
        IConvertToActionResult result = controller.ToActionResult(resultVariant);
        IActionResult actionResult = result.Convert();
        var statusCodeActionResult = (IStatusCodeActionResult)actionResult;
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ActionResult<ResponseObjectFake>>();
        statusCodeActionResult.StatusCode.Should().Be(expectedHttpCode);
    }

    public static IEnumerable<object[]> ResultTypeData = new List<object[]>
    {
        new object[] { Result.Ok(new ResponseObjectFake()), 200 },
        new object[] { Result.Fail<ResponseObjectFake>("Error"), 500 }
    };

    public record ResponseObjectFake;

    private class ControllerFake : ControllerBase;
}