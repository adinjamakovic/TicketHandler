using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Commands.Create
{
    public class CreateEventNewsCommandHandler(
        IAppDbContext ctx,
        IAppCurrentUser appCurrentUser,
        IImageStorage imageStorage)
        : IRequestHandler<CreateEventNewsCommand, int>
    {
        public async Task<int> Handle(CreateEventNewsCommand req, CancellationToken ct)
        {
            if (!appCurrentUser.IsOrganiser)
                throw new MarketBusinessRuleException("111", "Only an organiser can enter event news");

            var org = await ctx.Organizers.FirstOrDefaultAsync(x => x.UserId == appCurrentUser.UserId, ct);

            if (org is null)
                throw new MarketNotFoundException("No organizer found");

            var eventEntity = await ctx.Events
                .FirstOrDefaultAsync(x => x.Id == req.EventId, ct);

            if (eventEntity is null)
                throw new MarketNotFoundException($"Event with an Id of {req.EventId} does not exist");

            if (eventEntity.OrganizerId != org.Id)
                throw new MarketBusinessRuleException("111", "Only the organiser who owns the event can enter event news");

            var normalizedHeader = req.Header.Trim();
            var normalizedBody = req.Body?.Trim() ?? string.Empty;

            var EventNews = new EventNewsEntity
            {
                OrganizerId = org.Id,
                EventId=req.EventId,
                Header= normalizedHeader,
                Body=normalizedBody,
                Image = await imageStorage.SaveAsync(ImageStorageCategory.EventNews, req.Image, ct)
            };

            ctx.EventNews.Add(EventNews);
            await ctx.SaveChangesAsync(ct);

            return EventNews.Id;
        }
    }
}
