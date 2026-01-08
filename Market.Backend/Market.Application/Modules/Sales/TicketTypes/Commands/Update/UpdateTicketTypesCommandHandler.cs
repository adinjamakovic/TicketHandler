using Market.Application.Modules.Sales.TicketTypes.Commands.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.TicketTypes.Commands.Update
{
    public class UpdateTicketTypesCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<UpdateTicketTypesCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateTicketTypesCommand req, CancellationToken ct)
        {
            var entity = await ctx.TicketTypes
                .Where(x => x.Id == req.Id)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                throw new MarketNotFoundException("Ticket types does not exist");

            if (!appCurrentUser.IsOrganiser)
                throw new MarketBusinessRuleException("111","Only organisers can edit ticket types");

            req.Name = req.Name.Trim();

            if (string.IsNullOrWhiteSpace(req.Name))
                req.Name = entity.Name;

            entity.Name = req.Name;
            entity.Description = req.Description.Trim();

            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
