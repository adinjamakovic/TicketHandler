using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.TicketTypes.Commands.Queries.List
{
    public sealed class ListTicketTypesQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListTicketTypesQuery, PageResult<ListTicketTypesQueryDto>>
    {
        public async Task<PageResult<ListTicketTypesQueryDto>> Handle(ListTicketTypesQuery req, CancellationToken ct)
        {
            var q = ctx.TicketTypes.AsNoTracking();

            var searchTerm = req.Search?.Trim().ToLower() ?? string.Empty;

            if(!string.IsNullOrWhiteSpace(searchTerm))
            {
                q = q.Where(x => x.Name.ToLower().Contains(searchTerm));
            }

            var projectedQuery = q.OrderBy(x => x.Name)
                .Select(x => new ListTicketTypesQueryDto
                {
                    Id=x.Id,
                    Name=x.Name,
                    Description=x.Description
                });

            return await PageResult<ListTicketTypesQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
