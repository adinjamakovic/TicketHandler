using Market.Application.Modules.Sales.TicketTypes.Commands.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.TicketTypes.Commands.Queries.GetById
{
    public class GetTicketTypesByIdQueryHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<GetTicketTypesByIdQuery, GetTicketTypesByIdQueryDto>
    {
        public async Task<GetTicketTypesByIdQueryDto> Handle(GetTicketTypesByIdQuery req, CancellationToken ct)
        {
            var TicketTypes = await ctx.TicketTypes
                .Where(x => x.Id == req.Id)
                .Select(x => new GetTicketTypesByIdQueryDto
                {
                    Name = x.Name,
                    Description = x.Description,
                })
                .FirstOrDefaultAsync(ct);
            if (TicketTypes is null)
                throw new MarketNotFoundException("Ticket types not found");

            return TicketTypes;
        }
    }
}
