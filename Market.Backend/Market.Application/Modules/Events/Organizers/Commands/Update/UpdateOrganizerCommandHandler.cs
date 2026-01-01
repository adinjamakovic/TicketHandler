using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Commands.Update
{
    public class UpdateOrganizerCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<UpdateOrganizerCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateOrganizerCommand req, CancellationToken ct)
        {
            if (appCurrentUser.IsUser)
                throw new MarketBusinessRuleException("111", "User can not edit organisers... How did we get here??");
            
            var entity = await ctx.Organizers
                .Where(x => x.Id == req.Id)
                .FirstOrDefaultAsync(ct);

            if (entity is null)
                throw new MarketNotFoundException("Invalid Organizer");

            var exists = await ctx.Organizers
                .AnyAsync(x => x.Id != req.Id && x.Name.ToLower() == req.Name.ToLower(), ct);

            if (exists)
                throw new MarketConflictException("Name already exists");
            
            var city = await ctx.Cities
                .Where(x=>x.Id == req.CityId)
                .FirstOrDefaultAsync(ct);

            if (city is null || city.IsDeleted == true)
                throw new MarketNotFoundException("The selected city does not exist");

            entity.Name = req.Name.Trim();
            entity.Description = req.Description?.Trim();
            entity.Address = req.Address.Trim();
            entity.CityId= req.CityId;

            await ctx.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
