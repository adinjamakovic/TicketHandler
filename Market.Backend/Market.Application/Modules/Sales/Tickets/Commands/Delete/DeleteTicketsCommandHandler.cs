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
            if (appCurrent.IsUser)
                throw new MarketBusinessRuleException("111", "A user can not issue a delete statement on an ticket... how did we get here?");

            var Tickets = await ctx.Tickets
                .FirstOrDefaultAsync(x => x.Id == req.Id, ct);

            if (Tickets is null)
                throw new MarketNotFoundException($"Ticket with Id {req.Id} does not exist");

            ctx.Tickets.Remove(Tickets);
            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
