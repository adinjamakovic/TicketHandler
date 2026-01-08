using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.TicketTypes.Commands.Create
{
    public class CreateTicketTypeCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<CreateTicketTypesCommand, int>
    {
        public async Task<int> Handle(CreateTicketTypesCommand req, CancellationToken ct)
        {
            if (!appCurrentUser.IsOrganiser)
                throw new MarketBusinessRuleException("111", "Only an organiser can enter ticket types");

            if (string.IsNullOrWhiteSpace(req.Name))
                throw new ValidationException("Name is required");

            var TicketType = new TicketTypeEntity
            {
                Name=req.Name,
                Description=req.Description,
                CreatedAtUtc = DateTime.UtcNow
            };

            ctx.TicketTypes.Add(TicketType);
            await ctx.SaveChangesAsync(ct);

            return TicketType.Id;
        }
    }
}
