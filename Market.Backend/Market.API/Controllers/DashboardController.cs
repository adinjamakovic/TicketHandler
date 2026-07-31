using Market.Application.Modules.Dashboard.Dashboard.Query.Get;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;
[ApiController]
[Route("[controller]")]
public class DashboardController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<GetDashboardQueryDto> GetDashboard(CancellationToken ct)
    {
        return await SendTraced(new GetDashboardQuery(), ct);
    }
}