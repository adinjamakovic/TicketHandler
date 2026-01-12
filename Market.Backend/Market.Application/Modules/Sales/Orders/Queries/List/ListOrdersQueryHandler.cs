using Market.Application.Modules.Sales.Orders.Queries.GetById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.List
{
    public sealed class ListOrdersQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListOrdersQuery, PageResult<ListOrdersQueryDto>>
    {
        public async Task<PageResult<ListOrdersQueryDto>> Handle(ListOrdersQuery req, CancellationToken ct)
        {
            var q = ctx.Orders.AsNoTracking();

            var searchTerm = req.Id;

            var projectedQuery = q.OrderBy(x => x.CreatedAtUtc)
                .Select(x => new ListOrdersQueryDto
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    Items = x.OrderItems.Select(y => new ListOrderOrderItems
                    {
                        Id = y.Id,
                        OrderId = y.OrderId,
                        TicketId = y.TicketId,
                        Quantity = y.Quantity,
                        Subtotal = y.Subtotal,
                        DiscountAmount = y.DiscountAmount,
                        DiscountPercent = y.DiscountPercent,
                        Total = y.Total
                    }).ToList()
                });

            return await PageResult<ListOrdersQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
