using Market.Application.Modules.Events.EventsNews.Queries.List;
using Market.Application.Modules.Events.EventType.Queries.List;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class EventTypesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListEventTypesQueryDto>> List([FromQuery] ListEventTypesQuery q, CancellationToken ct)
    {
        var result = await sender.Send(q, ct);
        return result;
    }
    
}