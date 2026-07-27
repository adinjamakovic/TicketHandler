using Market.Application.Modules.Geography.Cities.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CitiesController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListCitiesQueryDto>> List([FromQuery] ListCitiesQuery query, CancellationToken ct)
    {
        var result = await SendTraced(query, ct);
        return result;
    }
}