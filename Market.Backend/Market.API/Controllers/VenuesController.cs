
using Market.Application.Modules.Geographical.Venues.Queries.List;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VenuesController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListVenuesQueryDto>> List([FromQuery] ListVenuesQuery query, CancellationToken ct)
    {
        var result = await SendTraced(query, ct);

        return result;
    }
}