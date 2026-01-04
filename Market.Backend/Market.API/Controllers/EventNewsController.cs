using Market.Application.Modules.Events.Events.Commands.Delete;
using Market.Application.Modules.Events.Events.Commands.Update;
using Market.Application.Modules.Events.EventsNews.Commands.Create;
using Market.Application.Modules.Events.EventsNews.Commands.Delete;
using Market.Application.Modules.Events.EventsNews.Commands.Update;
using Market.Application.Modules.Events.EventsNews.Queries.GetById;
using Market.Application.Modules.Events.EventsNews.Queries.List;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class EventNewsController(ISender sender) : ControllerBase
{
    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteEventNewsCommand { Id = id}, ct);
    }
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

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateEventNewsCommand command, CancellationToken ct)
    {
        command.Id=id;
        await sender.Send(command, ct);
    }
}