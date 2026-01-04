using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Queries.GetById
{
    public class GetEventNewsByIdQueryHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<GetEventNewsByIdQuery, GetEventNewsByIdQueryDto>
    {
        public async Task<GetEventNewsByIdQueryDto> Handle(GetEventNewsByIdQuery req, CancellationToken ct)
        {
            var EventNews = await ctx.EventNews
                .Where(x => x.Id == req.Id)
                .Select(x => new GetEventNewsByIdQueryDto
                {
                    Id = x.Id,
                    Event = x.Event.Name,
                    Organizer = x.Organizer.Name,
                    Header = x.Header,
                    Body = x.Body ?? string.Empty,
                    Image = new byte[0]
                })
                .FirstOrDefaultAsync(ct);
            if (EventNews is null)
                throw new MarketNotFoundException("Event News not found");

            return EventNews;
        }
    }
}
