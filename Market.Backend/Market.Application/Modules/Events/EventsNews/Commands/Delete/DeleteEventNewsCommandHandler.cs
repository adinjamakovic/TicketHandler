using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Commands.Delete
{
    public class DeleteEventNewsCommandHandler(
        IAppDbContext ctx,
        IAppCurrentUser appCurrentUser,
        IImageStorage imageStorage)
        : IRequestHandler<DeleteEventNewsCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteEventNewsCommand req, CancellationToken ct)
        {
            var EventNews = await ctx.EventNews.
                Where(x => x.Id == req.Id)
                .FirstOrDefaultAsync(ct);

            var org = await ctx.Organizers
                .Where(x => x.UserId == appCurrentUser.UserId)
                .FirstOrDefaultAsync(ct);

            if (EventNews is null)
                throw new MarketNotFoundException("Event News not found");
            
            if (!appCurrentUser.IsOrganiser ||
                org is null ||
                EventNews.OrganizerId != org.Id)
                throw new MarketBusinessRuleException("111","You are not allowed to do this");

            ctx.EventNews.Remove(EventNews);
            await ctx.SaveChangesAsync(ct);

            if (!string.IsNullOrWhiteSpace(EventNews.Image))
                await imageStorage.DeleteIfExistsAsync(ImageStorageCategory.EventNews, EventNews.Image, ct);

            return Unit.Value;
        }
    }
}
