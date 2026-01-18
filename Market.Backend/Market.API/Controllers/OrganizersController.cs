using Market.Application.Modules.Events.Organizers.Commands.Create;
using Market.Application.Modules.Events.Organizers.Commands.Delete;
using Market.Application.Modules.Events.Organizers.Commands.Update;
using Market.Application.Modules.Events.Organizers.Queries.GetById;
using Market.Application.Modules.Events.Organizers.Queries.GetByUserId;
using Market.Application.Modules.Events.Organizers.Queries.List;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganizersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateOrganizerCommand cmd, CancellationToken ct)
    {
        int id = await sender.Send(cmd, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

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
    [HttpGet("UserId/{userId:int}")]
    public async Task<GetOrganizerByUserIdQueryDto> GetByUserId(int userId, CancellationToken ct)
    {
        var Organizer = await sender.Send(new GetOrganizerByUserIdQuery { UserId = userId }, ct);
        return Organizer;
    }
    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateOrganizerCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteOrganizerCommand { Id = id }, ct);
    }
    
}