using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Performers.Queries.GetById
{
    public sealed class GetPerformerByIdQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetPerformerByIdQuery, GetPerformerByIdQueryDto>
    {
        public async Task<GetPerformerByIdQueryDto> Handle(GetPerformerByIdQuery req, CancellationToken ct)
        {
            var q = ctx.Performers
                .Where(x => x.Id == req.Id);

            var dto = await q.Select(x=> new GetPerformerByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Genre=x.Genre.Name
            }).FirstOrDefaultAsync(ct);

            if (dto is null)
                throw new MarketNotFoundException($"Performer with Id of {req.Id} not found");
            return dto;
        }
    }
}
