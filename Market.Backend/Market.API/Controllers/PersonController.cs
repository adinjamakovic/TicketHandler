using Market.Application.Modules.Identity.Person.Commands.Create;
using Market.Application.Modules.Identity.Person.Queries.GetById;
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
public class PersonController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreatePersonCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
    [HttpGet("{id:int}")]
    public async Task<GetPersonByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var Person = await sender.Send(new GetPersonByIdQuery { Id = id }, ct);
    
        return Person;
    }
}