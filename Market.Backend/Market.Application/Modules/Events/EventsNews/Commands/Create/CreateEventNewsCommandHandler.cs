using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Commands.Create
{
    public class CreateEventNewsCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<CreateEventNewsCommand, int>
    {
        public async Task<int> Handle(CreateEventNewsCommand req, CancellationToken ct)
        {
            if (!appCurrentUser.IsOrganiser)
                throw new MarketBusinessRuleException("111", "Only an organiser can enter event news");

            var org = await ctx.Organizers.FirstOrDefaultAsync(x => x.UserId == appCurrentUser.UserId, ct);
            var normalizedHeader = req.Header.Trim();
            var normalizedBody = req.Body?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(normalizedHeader))
                throw new ValidationException("Header is required");

            var EventNews = new EventNewsEntity
            {
                OrganizerId = org.Id,
                EventId=req.EventId,
                Header= normalizedHeader,
                Body=normalizedBody,
                Image = new byte[0]
            };

            ctx.EventNews.Add(EventNews);
            await ctx.SaveChangesAsync(ct);

            return EventNews.Id;
        }
    }
}
