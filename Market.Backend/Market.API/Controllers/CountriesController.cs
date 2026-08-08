using Market.Application.Modules.Geography.Countries.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CountriesController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListCountriesQueryDto>> List([FromQuery] ListCountriesQuery query, CancellationToken ct)
    {
        var result = await SendTraced(query, ct);
        return result;
    }
}
