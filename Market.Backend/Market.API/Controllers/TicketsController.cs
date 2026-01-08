using Market.Application.Modules.Events.Events.Commands.Create;
using Market.Application.Modules.Events.Events.Commands.Delete;
using Market.Application.Modules.Events.Events.Commands.Update;
using Market.Application.Modules.Events.Events.Queries.GetById;
using Market.Application.Modules.Events.Events.Queries.List;
using Market.Application.Modules.Sales.Tickets.Commands.Create;
using Market.Application.Modules.Sales.Tickets.Commands.Delete;
using Market.Application.Modules.Sales.Tickets.Commands.Update;
using Market.Application.Modules.Sales.Tickets.Queries.GetByEventId;
using Market.Application.Modules.Sales.Tickets.Queries.GetById;
using Market.Application.Modules.Sales.Tickets.Queries.List;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TicketsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTicketsCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListTicketsQueryDto>> List([FromQuery] ListTicketsQuery q, CancellationToken ct)
    {
        var result = await sender.Send(q, ct);
        return result;
    }
    [HttpGet("{id:int}")]
    public async Task<GetTicketsByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var dto = await sender.Send(new GetTicketsByIdQuery { Id = id }, ct);
        return dto;
    }

    [HttpGet("EventId")]
    public async Task<PageResult<GetTicketsByEventIdQueryDto>> ListTicketsByEventId([FromQuery] GetTicketsByEventIdQuery q, CancellationToken ct)
    {
        var result = await sender.Send(q, ct);
        return result;
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteTicketsCommand { Id = id }, ct); 
    }

    [HttpPut("id:int")]
    public async Task Update(int id, UpdateTicketsCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }
}