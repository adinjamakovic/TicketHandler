using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Commands.Update
{
    public class UpdateEventNewsCommandHandler(
        IAppDbContext ctx,
        IAppCurrentUser appCurrentUser,
        IImageStorage imageStorage)
        : IRequestHandler<UpdateEventNewsCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateEventNewsCommand req, CancellationToken ct)
        {
            var entity = await ctx.EventNews
                .Where(x => x.Id == req.Id)
                .FirstOrDefaultAsync(ct);

            if (entity == null)
                throw new MarketNotFoundException("Event news does not exist");

            if (!appCurrentUser.IsOrganiser)
                throw new MarketBusinessRuleException("111","Only organisers can edit EventNews");

            if (entity.OrganizerId !=
                (await ctx.Organizers.
                FirstOrDefaultAsync(x => x.UserId == appCurrentUser.UserId, ct))?.Id)
                throw new MarketBusinessRuleException("111", "Only the organiser who made the EventNews can edit them");

            if (!string.IsNullOrWhiteSpace(req.Header.Trim()))
                entity.Header = req.Header;
                

            entity.Body = req.Body.Trim();

            entity.Image = await imageStorage.ReplaceIfUploadedAsync(
                ImageStorageCategory.EventNews, entity.Image, req.Image, ct);

            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
