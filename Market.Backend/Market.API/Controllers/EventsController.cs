using Market.Application.Modules.Events.Events.Queries.List;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListEventsQueryDto>> List([FromQuery] ListEventsQuery q, CancellationToken ct)
    {
        var result = await sender.Send(q, ct);
        return result;
    }


    [HttpGet("with-performers")]
    public async Task<PageResult<ListEventsWithPerformersQueryDto>> ListWithPerformers([FromQuery] ListEventsWithPerformersQuery q, CancellationToken ct)
    {
        var result = await sender.Send(q, ct);
        return result;
    }
}