using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Commands.Delete
{
    public class DeleteTicketsCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrent)
        : IRequestHandler<DeleteTicketsCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteTicketsCommand req, CancellationToken ct)
        {
            if (!appCurrent.IsOrganiser)
                throw new MarketBusinessRuleException("111", "Only an organiser can delete tickets");

            var organizerId = await ctx.Organizers
                .Where(x => x.UserId == appCurrent.UserId)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(ct);

            if (organizerId is null)
                throw new MarketNotFoundException("No organizer found");

            var Tickets = await ctx.Tickets
                .FirstOrDefaultAsync(x => x.Id == req.Id, ct);

            if (Tickets is null)
                throw new MarketNotFoundException($"Ticket with Id {req.Id} does not exist");

            // Only the organizer running the event the ticket was issued for may remove it.
            if (!await ctx.Events.AnyAsync(x => x.Id == Tickets.EventId && x.OrganizerId == organizerId, ct))
                throw new MarketBusinessRuleException("111", $"Ticket with Id {req.Id} belongs to another organizer");

            ctx.Tickets.Remove(Tickets);
            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
