using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Commands.Create
{
    public class CreateOrganizerCommandHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<CreateOrganizerCommand, int>
    {
        public async Task<int> Handle(CreateOrganizerCommand req, CancellationToken ct)
        {
            if (!appCurrentUser.IsAdmin)
                throw new MarketBusinessRuleException("111", "Only an admin can add an organizer");

            var normalized = req.Name?.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
                throw new ValidationException("Name is required.");

            bool exists = await ctx.Organizers
                .AnyAsync(x => x.Name == normalized, ct);

            if (exists)
                throw new MarketConflictException("This organizer already exists");

            var hasher = new PasswordHasher<PersonEntity>();

            var User = new PersonEntity
            {
                FirstName = req.User.FirstName,
                LastName = req.User.LastName,
                BirthDate = req.User.BirthDate,
                CityId = req.User.CityId,
                Address = req.User.Address,
                Phone = req.User.Phone,
                Email = req.User.Email,
                PasswordHash = hasher.HashPassword(null!, req.User.Password),
                IsAdmin = false,
                IsUser = false,
                IsOrganiser = true,
                IsEnabled = true
            };

            ctx.Persons.Add(User);

            var city = await ctx.Cities
                .Where(x=>x.Id==req.CityId)
                .FirstOrDefaultAsync(ct);

            if (city is null)
                throw new ValidationException("Invalid city");

            var Organizer = new OrganizerEntity
            {
                Name = req.Name!.Trim(),
                Description = req.Description?.Trim(),
                Address = req.Address.Trim(),
                CityId = req.CityId,
                Logo = String.Empty,
                User = User,
                IsDeleted = false,
                CreatedAtUtc = DateTime.UtcNow
            };

            ctx.Organizers.Add(Organizer);
            await ctx.SaveChangesAsync(ct);

            return Organizer.Id;
        }
    }
}
