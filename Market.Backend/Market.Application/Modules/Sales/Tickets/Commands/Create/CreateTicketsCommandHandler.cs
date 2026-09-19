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

            var organizerId = await ctx.Organizers
                .Where(x => x.UserId == appCurrentUser.UserId)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(ct);

            if (organizerId is null)
                throw new MarketNotFoundException("No organizer found");

            var eventEntity = await ctx.Events
                .FirstOrDefaultAsync(x => x.Id == req.EventId, ct);

            if (eventEntity is null)
                throw new MarketNotFoundException($"Event with Id {req.EventId} does not exist");

            // Tickets are inventory on someone's event, so the caller has to own that event.
            if (eventEntity.OrganizerId != organizerId)
                throw new MarketBusinessRuleException("111", $"Event with Id {req.EventId} belongs to another organizer");

            var Tickets = new TicketsEntity
            {
                EventId = req.EventId,
                TicketTypeId = req.TicketTypeId,
                QuantityInStock = req.QuantityInStock,
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
