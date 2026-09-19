using Market.Application.Modules.Identity.Person.Commands.Create;
using Market.Application.Modules.Identity.Person.Commands.Update;
using Market.Application.Modules.Identity.Person.Commands.UpdateRoles;
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
public class PersonController(ISender sender) : ApiControllerBase(sender)
{
    /// <summary>
    /// Creates a regular user account. The roles are set by the server, not by the request -
    /// use <see cref="UpdateRoles"/> to grant admin or organiser rights.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreatePersonCommand command, CancellationToken ct)
    {
        int id = await SendTraced(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
    [HttpGet("{id:int}")]
    public async Task<GetPersonByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var Person = await SendTraced(new GetPersonByIdQuery { Id = id }, ct);

        return Person;
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdatePersonCommand command, CancellationToken ct)
    {
        command.Id = id;
        await SendTraced(command, ct);
    }

    /// <summary>
    /// Assigns the security roles of a person. Admin only - the handler rejects everybody else.
    /// </summary>
    [HttpPut("{id:int}/roles")]
    public async Task UpdateRoles(int id, UpdatePersonRolesCommand command, CancellationToken ct)
    {
        command.Id = id;
        await SendTraced(command, ct);
    }
}