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

            var User = await ctx.Persons
                .Where(x => x.Id == Organizer!.UserId)
                .FirstOrDefaultAsync();

            if (Organizer == null)
                throw new MarketNotFoundException("Organizer was not found");

            if(User == null)
                throw new MarketNotFoundException("User associated with the organizer was not found");

            ctx.Organizers.Remove(Organizer);
            ctx.Persons.Remove(User);
            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
