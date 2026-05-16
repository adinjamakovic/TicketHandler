using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Geography.Cities.Queries.List
{
    public sealed class ListCitiesQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListCitiesQuery, PageResult<ListCitiesQueryDto>>
    {
        public async Task<PageResult<ListCitiesQueryDto>> Handle(ListCitiesQuery req, CancellationToken ct)
        {
            var q = ctx.Cities.AsNoTracking();

            var searchTerm = req.Search?.Trim().ToLower() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(searchTerm))
                q = q.Where(x => x.Name.ToLower().Contains(searchTerm));

            var projectedQ = q.OrderBy(x => x.Name)
                .Select(x => new ListCitiesQueryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PostalCode = x.PostalCode,
                });

            return await PageResult<ListCitiesQueryDto>.FromQueryableAsync(projectedQ, req.Paging, ct);
        }
    }
}
