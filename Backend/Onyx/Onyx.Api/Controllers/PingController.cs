using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Onyx.Api.Controllers;

/// <summary>
/// Generic ping/pong API
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class PingController(ILogger<PingController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        logger.LogTrace("Ping/Pong");
        return Ok("Pong");
    }
}