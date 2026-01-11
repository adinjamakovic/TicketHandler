using Market.Application.Modules.Events.Events.Commands.Create;
using Market.Application.Modules.Events.Events.Commands.Delete;
using Market.Application.Modules.Events.Events.Commands.Update;
using Market.Application.Modules.Events.Events.Queries.GetById;
using Market.Application.Modules.Events.Events.Queries.List;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateEventCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListEventsQueryDto>> List([FromQuery] ListEventsQuery q, CancellationToken ct)
    {
        var result = await sender.Send(q, ct);
        return result;
    }
    [HttpGet("{id:int}")]
    public async Task<GetEventByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var dto = await sender.Send(new GetEventByIdQuery { Id = id }, ct);
        return dto;
    }

    [HttpGet("with-performers")]
    public async Task<PageResult<ListEventsWithPerformersQueryDto>> ListWithPerformers([FromQuery] ListEventsWithPerformersQuery q, CancellationToken ct)
    {
        var result = await sender.Send(q, ct);
        return result;
    }

    [HttpGet("OrganizerId")]
    public async Task<PageResult<GetEventsByOrganizerIdQueryDto>> ListEventsByOrganizerId([FromQuery] GetEventsByOrganizerIdQuery q, CancellationToken ct)
    {
        var result = await sender.Send(q, ct);
        return result;
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteEventCommand { Id = id }, ct); 
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateEventCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }
}