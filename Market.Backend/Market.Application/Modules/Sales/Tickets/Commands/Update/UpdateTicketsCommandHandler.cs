using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Commands.Update
{
    public class UpdateTicketsCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<UpdateTicketsCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateTicketsCommand req, CancellationToken ct)
        {
            #region Validation
            if (!appCurrentUser.IsOrganiser)
                throw new MarketBusinessRuleException("111", "Only an organiser can edit tickets");

            if (await ctx.TicketTypes.FirstOrDefaultAsync(x => x.Id == req.TicketTypeId, ct) is null)
                throw new MarketNotFoundException("Ticket type does not exist");

            var organizerId = await ctx.Organizers
                .Where(x => x.UserId == appCurrentUser.UserId)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(ct);

            if (organizerId is null)
                throw new MarketNotFoundException("No organizer found");

            var Tickets = await ctx.Tickets
                .Where(x => x.Id == req.Id)
                .FirstOrDefaultAsync(ct);

            if (Tickets is null)
                throw new MarketNotFoundException("This ticket does not exist");

            // The ticket being edited has to sit on an event the caller owns...
            if (!await ctx.Events.AnyAsync(x => x.Id == Tickets.EventId && x.OrganizerId == organizerId, ct))
                throw new MarketBusinessRuleException("111", $"Ticket with Id {req.Id} belongs to another organizer");

            var targetEvent = await ctx.Events
                .FirstOrDefaultAsync(x => x.Id == req.EventId, ct);

            if (targetEvent is null)
                throw new MarketNotFoundException("Event does not exist");

            // ...and so does the event it is being (re)assigned to, otherwise an organizer could
            // push their own ticket into somebody else's event.
            if (targetEvent.OrganizerId != organizerId)
                throw new MarketBusinessRuleException("111", $"Event with Id {req.EventId} belongs to another organizer");
            #endregion

            Tickets.EventId = req.EventId;
            Tickets.TicketTypeId = req.TicketTypeId;
            Tickets.QuantityInStock = req.QuantityInStock;
            Tickets.UnitPrice = req.UnitPrice;
            Tickets.Benefits = req.Benefits;
            
           
            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
