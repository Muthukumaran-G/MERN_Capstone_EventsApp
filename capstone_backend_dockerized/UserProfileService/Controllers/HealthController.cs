using Microsoft.AspNetCore.Mvc;

namespace UserProfileService.Controllers
{
    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("Healthy");
    }

}
