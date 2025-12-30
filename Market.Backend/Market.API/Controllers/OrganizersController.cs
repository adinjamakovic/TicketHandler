using Market.Application.Modules.Events.Organizers.Queries.List;

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

}