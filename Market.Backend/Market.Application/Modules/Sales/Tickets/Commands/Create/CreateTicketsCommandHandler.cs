using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Commands.Create
{
    public class CreateTicketsCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<CreateTicketsCommand, int>
    {
        public async Task<int> Handle(CreateTicketsCommand req, CancellationToken ct)
        {
            if (!appCurrentUser.IsOrganiser)
                throw new MarketBusinessRuleException("111", "Only an organiser can create tickets");

            var Tickets = new TicketsEntity
            {
                EventId = req.EventId,
                TicketTypeId = req.TicketTypeId,
                QuanityInStock = req.QuanityInStock,
                UnitPrice = req.UnitPrice,
                Benefits = req.Benefits,
                CreatedAtUtc = DateTime.UtcNow
            };

            ctx.Tickets.Add(Tickets);
            await ctx.SaveChangesAsync(ct);

            return Tickets.Id;
        }
    }
}
