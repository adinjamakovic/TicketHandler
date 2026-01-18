using Market.Application.Modules.Sales.TicketTypes.Commands.Create;
using Market.Application.Modules.Sales.TicketTypes.Commands.Delete;
using Market.Application.Modules.Sales.TicketTypes.Commands.Queries.GetById;
using Market.Application.Modules.Sales.TicketTypes.Commands.Queries.List;
using Market.Application.Modules.Sales.TicketTypes.Commands.Update;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TicketTypeController(ISender sender) : ControllerBase
{
    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteTicketTypesCommand { Id = id}, ct);
    }
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTicketTypesCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
    
        return CreatedAtAction(nameof(GetById),new { id }, new { id });
    }
    
    [HttpGet]
    public async Task<PageResult<ListTicketTypesQueryDto>> List([FromQuery] ListTicketTypesQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
    
        return result;
    }
    
    [HttpGet("{id:int}")]
    public async Task<GetTicketTypesByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var EventNews = await sender.Send(new GetTicketTypesByIdQuery { Id = id }, ct);
    
        return EventNews;
    }
    
    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateTicketTypesCommand command, CancellationToken ct)
    {
        command.Id=id;
        await sender.Send(command, ct);
    }
}