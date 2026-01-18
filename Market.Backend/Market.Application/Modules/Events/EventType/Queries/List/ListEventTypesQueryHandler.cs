using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventType.Queries.List
{
    public sealed class ListEventTypesQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListEventTypesQuery, PageResult<ListEventTypesQueryDto>>
    {
        public async Task<PageResult<ListEventTypesQueryDto>> Handle(ListEventTypesQuery req, CancellationToken ct)
        {
            var q = ctx.EventTypes.AsNoTracking();

            var searchTerm = req.Search?.ToLower().Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(searchTerm))
                q = q.Where(x => x.Name.ToLower().Contains(searchTerm));

            var projectedQuery = q.OrderBy(x => x.Name)
                .Select(x => new ListEventTypesQueryDto
                {
                    Id = x.Id,
                    Name = x.Name
                });

            return await PageResult<ListEventTypesQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
