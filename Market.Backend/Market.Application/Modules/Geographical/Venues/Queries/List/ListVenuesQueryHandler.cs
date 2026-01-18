using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Geographical.Venues.Queries.List
{
    public class ListVenuesQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListVenuesQuery, PageResult<ListVenuesQueryDto>>
    {
        public async Task<PageResult<ListVenuesQueryDto>> Handle(ListVenuesQuery req, CancellationToken ct)
        {
            var q = ctx.Venues.AsNoTracking();

            var searchTerm = req.Search?.ToLower().Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(searchTerm))
                q = q.Where(x => x.Name.ToLower().Contains(searchTerm));

            var projectedQuery = q.OrderBy(x => x.Name)
                .Select(x => new ListVenuesQueryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Seated = x.Seated,
                    Standing = x.Standing,
                    Location = x.Location.Name
                });

            return await PageResult<ListVenuesQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
