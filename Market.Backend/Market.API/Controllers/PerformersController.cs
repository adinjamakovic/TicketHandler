using Market.Application.Modules.Events.Performers.Queries.GetById;
using Market.Application.Modules.Events.Performers.Queries.List;
using Microsoft.EntityFrameworkCore.Query;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PerformersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListPerformersQueryDto>> List([FromQuery] ListPerformersQuery q, CancellationToken ct)
    {
        var result = await sender.Send(q, ct);
        return result;
    }
    [HttpGet("{id:int}")]
    public async Task<GetPerformerByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var dto = await sender.Send(new GetPerformerByIdQuery { Id = id }, ct);
        return dto;
    }
}