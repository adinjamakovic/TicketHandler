using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Commands.Delete
{
    public class DeleteEventCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrent)
        : IRequestHandler<DeleteEventCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteEventCommand req, CancellationToken ct)
        {
            if (appCurrent.IsUser)
                throw new MarketBusinessRuleException("111", "A user can not issue a delete statement on an event... how did we get here?");

            var Event = await ctx.Events
                .FirstOrDefaultAsync(x => x.Id == req.Id, ct);

            if (Event is null)
                throw new MarketNotFoundException($"Event with Id {req.Id} does not exist");

            ctx.Events.Remove(Event);

            var PerformerEvents = await ctx.PerformerEvents
                .Where(x => x.EventId == req.Id)
                .ToListAsync(ct);

            if(PerformerEvents != null)
            {
                foreach (var PerformerEvent in PerformerEvents)
                    ctx.PerformerEvents.Remove(PerformerEvent);
            }

            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
