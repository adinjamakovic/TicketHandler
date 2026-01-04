using Market.Application.Modules.Events.EventsNews.Commands.Create;
using Market.Application.Modules.Events.EventsNews.Queries.GetById;
using Market.Application.Modules.Events.EventsNews.Queries.List;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class EventNewsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateEventNewsCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById),new { id }, new { id });
    }

    [HttpGet]
    public async Task<PageResult<ListEventNewsQueryDto>> List([FromQuery] ListEventNewsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);

        return result;
    }

    [HttpGet("{id:int}")]
    public async Task<GetEventNewsByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var EventNews = await sender.Send(new GetEventNewsByIdQuery { Id = id }, ct);

        return EventNews;
    }
}