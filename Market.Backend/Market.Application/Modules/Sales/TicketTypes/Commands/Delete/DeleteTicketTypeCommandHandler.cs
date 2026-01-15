using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.TicketTypes.Commands.Delete
{
    public class DeleteTicketTypesCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<DeleteTicketTypesCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteTicketTypesCommand req, CancellationToken ct)
        {
            var TicketTypes = await ctx.TicketTypes.
                Where(x => x.Id == req.Id)
                .FirstOrDefaultAsync(ct);
            if (TicketTypes is null)
                throw new MarketNotFoundException("Ticket types not found");
            
            if (appCurrentUser.IsUser)
                throw new MarketBusinessRuleException("111","You are not allowed to do this");

            ctx.TicketTypes.Remove(TicketTypes);
            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
