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
            if (appCurrentUser.IsUser)
                throw new MarketBusinessRuleException("111", "A user can not edit tickets ... how did we get here?");

            if (await ctx.Events.FirstOrDefaultAsync(x => x.Id == req.EventId, ct) is null)
                throw new MarketNotFoundException("Event does not exist");

            if (await ctx.TicketTypes.FirstOrDefaultAsync(x => x.Id == req.TicketTypeId, ct) is null)
                throw new MarketNotFoundException("Ticket type does not exist");
            #endregion
            var Tickets = await ctx.Tickets
                .Where(x => x.Id == req.Id)
                .FirstOrDefaultAsync(ct);

            if (Tickets == null)
                throw new MarketNotFoundException("This ticket does not exist");

            Tickets.EventId = req.EventId;
            Tickets.TicketTypeId = req.TicketTypeId;
            Tickets.QuanityInStock = req.QuanityInStock;
            Tickets.UnitPrice = req.UnitPrice;
            Tickets.Benefits = req.Benefits;
            
           
            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
