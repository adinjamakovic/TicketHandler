
using Market.Application.Modules.Geographical.Venues.Queries.List;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VenuesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListVenuesQueryDto>> List([FromQuery] ListVenuesQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);

        return result;
    }
}