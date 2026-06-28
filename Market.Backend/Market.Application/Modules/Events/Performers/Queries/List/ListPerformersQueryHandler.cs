using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Performers.Queries.List
{
    public sealed class ListPerformersQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListPerformersQuery, PageResult<ListPerformersQueryDto>>
    {
        public async Task<PageResult<ListPerformersQueryDto>> Handle(ListPerformersQuery req, CancellationToken ct)
        {
            var q = ctx.Performers.AsNoTracking();

            var searchTerm = req.Search?.ToLower().Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(searchTerm))
                q = q.Where(x => x.Name.ToLower().Contains(searchTerm));

            var projectedQuery = q.OrderBy(x => x.Name)
                .Select(x => new ListPerformersQueryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Genre = x.Genre.Name
                });

            return await PageResult<ListPerformersQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
