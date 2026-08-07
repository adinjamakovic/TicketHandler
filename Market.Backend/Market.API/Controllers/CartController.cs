using Market.Application.Modules.Sales.Cart.Commands.AddItem;
using Market.Application.Modules.Sales.Cart.Commands.Clear;
using Market.Application.Modules.Sales.Cart.Commands.RemoveItem;
using Market.Application.Modules.Sales.Cart.Commands.UpdateItem;
using Market.Application.Modules.Sales.Cart.Queries.Get;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CartController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<GetCartQueryDto> Get(CancellationToken ct)
    {
        var dto = await SendTraced(new GetCartQuery(), ct);
        return dto;
    }

    [HttpPost("items")]
    public async Task AddItem(AddCartItemCommand command, CancellationToken ct)
    {
        await SendTraced(command, ct);
    }

    [HttpPut("items/{ticketId:int}")]
    public async Task UpdateItem(int ticketId, UpdateCartItemCommand command, CancellationToken ct)
    {
        command.TicketId = ticketId;
        await SendTraced(command, ct);
    }

    [HttpDelete("items/{ticketId:int}")]
    public async Task RemoveItem(int ticketId, CancellationToken ct)
    {
        await SendTraced(new RemoveCartItemCommand { TicketId = ticketId }, ct);
    }

    [HttpDelete]
    public async Task Clear(CancellationToken ct)
    {
        await SendTraced(new ClearCartCommand(), ct);
    }
}
