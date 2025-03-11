using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/gateway")]
public class ApiGatewayController : ControllerBase
{
    [HttpGet("protected-route")]
    public IActionResult ProtectedRoute()
    {
        return Ok(new { message = "You have access!" });
    }
}