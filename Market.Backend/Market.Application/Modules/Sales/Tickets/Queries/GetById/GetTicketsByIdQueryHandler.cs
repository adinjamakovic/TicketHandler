using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Queries.GetById
{
    public sealed class GetTicketsByIdQueryHandler(IAppDbContext ctx)
        : IRequestHandler<GetTicketsByIdQuery, GetTicketsByIdQueryDto>
    {
        public async Task<GetTicketsByIdQueryDto> Handle(GetTicketsByIdQuery req, CancellationToken ct)
        {
            var q = ctx.Tickets
                .Where(x => x.Id == req.Id);

            
            var dto = await q.Select(x => new GetTicketsByIdQueryDto
            {
               Id = x.Id,
               EventId = x.EventId,
               TicketTypeId = x.TicketTypeId,
               QuanityInStock = x.QuanityInStock,
               UnitPrice = x.UnitPrice,
               Benefits = x.Benefits,
            }).FirstOrDefaultAsync(ct);

            if (dto is null)
                throw new MarketNotFoundException($"Ticket with Id {req.Id} not found");
            return dto;
        }
    }
}
