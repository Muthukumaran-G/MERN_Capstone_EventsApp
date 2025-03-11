using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents()
    {
        var events = await _eventService.GetEventsAsync();
        if (events == null || events.Count == 0)
            return NotFound("No events found.");

        return Ok(events);
    }
}