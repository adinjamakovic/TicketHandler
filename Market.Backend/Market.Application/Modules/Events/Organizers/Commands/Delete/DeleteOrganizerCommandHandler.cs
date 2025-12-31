using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Commands.Delete
{
    public class DeleteOrganizerCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<DeleteOrganizerCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteOrganizerCommand req, CancellationToken ct)
        {
            if (!appCurrentUser.IsAdmin)
                throw new MarketBusinessRuleException("111", "Only an admin can delete an organizer");

            var Organizer = await ctx.Organizers
                .Where(x => x.Id == req.Id)
                .FirstOrDefaultAsync();

            if (Organizer == null)
                throw new MarketNotFoundException("Organizer was not found");

            ctx.Organizers.Remove(Organizer);
            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
