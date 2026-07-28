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
public class EventNewsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await SendTraced(new DeleteEventNewsCommand { Id = id}, ct);
    }
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<int>> Create([FromForm] CreateEventNewsCommand command, CancellationToken ct)
    {
        int id = await SendTraced(command, ct);

        return CreatedAtAction(nameof(GetById),new { id }, new { id });
    }

    [HttpGet]
    public async Task<PageResult<ListEventNewsQueryDto>> List([FromQuery] ListEventNewsQuery query, CancellationToken ct)
    {
        var result = await SendTraced(query, ct);

        return result;
    }

    [HttpGet("{id:int}")]
    public async Task<GetEventNewsByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var EventNews = await SendTraced(new GetEventNewsByIdQuery { Id = id }, ct);

        return EventNews;
    }

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task Update(int id, [FromForm] UpdateEventNewsCommand command, CancellationToken ct)
    {
        command.Id=id;
        await SendTraced(command, ct);
    }
}