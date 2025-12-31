using Market.Application.Modules.Events.Organizers.Commands.Create;
using Market.Application.Modules.Events.Organizers.Commands.Delete;
using Market.Application.Modules.Events.Organizers.Queries.GetById;
using Market.Application.Modules.Events.Organizers.Queries.List;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganizersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListOrganizersQueryDto>> List([FromQuery] ListOrganizersQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }

    [HttpGet("{id:int}")]
    public async Task<GetOrganizerByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var Organizer = await sender.Send(new GetOrganizerByIdQuery { Id = id }, ct);
        return Organizer;
    }
 
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateOrganizerCommand cmd, CancellationToken ct)
    {
        int id = await sender.Send(cmd, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteOrganizerCommand { Id = id }, ct);
    }
    
}