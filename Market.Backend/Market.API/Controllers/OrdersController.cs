using Market.Application.Modules.Events.Events.Commands.Create;
using Market.Application.Modules.Events.Events.Commands.Delete;
using Market.Application.Modules.Events.Events.Commands.Update;
using Market.Application.Modules.Events.Events.Queries.GetById;
using Market.Application.Modules.Events.Events.Queries.List;
using Market.Application.Modules.Sales.Orders.Commands.Create;
using Market.Application.Modules.Sales.Orders.Commands.Delete;
using Market.Application.Modules.Sales.Orders.Queries.GetById;
using Market.Application.Modules.Sales.Orders.Queries.GetByPersonId;
using Market.Application.Modules.Sales.Orders.Queries.List;
using Market.Application.Modules.Sales.Orders.Queries.ListWithTickets;
using Market.Application.Modules.Sales.Orders.Queries.ListWithTicketsAndEvents;
using Market.Application.Sales.Sales.Orders.Commands.Update;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateOrderCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListOrdersQueryDto>> List([FromQuery] ListOrdersQuery q, CancellationToken ct)
    {
        var result = await sender.Send(q, ct);
        return result;
    }
    [HttpGet("{id:int}")]
    public async Task<GetOrderByIdQueryDto> GetById(int id, CancellationToken ct)
    { 
        var dto = await sender.Send(new GetOrderByIdQuery { Id = id }, ct);
        return dto;
    }
    
    [HttpGet("with-tickets")]
    public async Task<PageResult<ListOrdersWithTicketsQueryDto>> ListWithTickets([FromQuery] ListOrdersWithTicketsQuery q, CancellationToken ct)
    {
        var result = await sender.Send(q, ct);
        return result;
    }
    [HttpGet("with-events")]
    public async Task<PageResult<ListOrdersWithTicketsAndEventsQueryDto>> ListWithTicketsAndEvents([FromQuery] ListOrdersWithTicketsAndEventsQuery q, CancellationToken ct)
    {
        var result = await sender.Send(q, ct);
        return result;
    }

    [HttpGet("PersonId:int")]
    public async Task<PageResult<GetOrderByPersonIdQueryDto>> GetOrdersByPersonId(int PersonId, CancellationToken ct)
    {
        var dto = await sender.Send(new GetOrderByPersonIdQuery { PersonId = PersonId }, ct);
        return dto;
    }
    
    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteOrderCommand { Id = id }, ct); 
    }

    [HttpPut("id:int")]
    public async Task Update(int id, UpdateOrderCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }
}