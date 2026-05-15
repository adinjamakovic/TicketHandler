using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Identity.Person.Commands.Create
{
    public class CreatePersonCommandHandler(IAppDbContext ctx)
        : IRequestHandler<CreatePersonCommand, int>
    {
        public async Task<int> Handle(CreatePersonCommand req, CancellationToken ct)
        {
            #region Validations
            var normalizedUsername = req.Username?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedUsername))
                throw new ValidationException("Userame is required.");

            var normalizedEmail = req.Email?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedEmail))
                throw new ValidationException("Email is required.");


            bool exists = await ctx.Persons
                .AnyAsync(x => x.Email == normalizedEmail, ct);

            if (exists)
                throw new MarketConflictException("Use a different email");

            var city = await ctx.Cities
                .Where(x => x.Id == req.CityId)
                .FirstOrDefaultAsync(ct);

            if (city is null)
                throw new ValidationException("City doesnt exist");
            #endregion
            var hasher = new PasswordHasher<PersonEntity>();
            var Person = new PersonEntity
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                BirthDate = req.BirthDate,
                CityId = req.CityId,
                Address = req.Address,
                Gender = req.Gender,
                Phone = req.Phone,
                UserName = req.Username,
                Email = req.Email,
                PasswordHash = hasher.HashPassword(null!, req.Password),
                IsAdmin = req.IsAdmin,
                IsOrganiser = req.isOrganiser,
                IsUser = req.isUser,
                IsEnabled = true
            };

            ctx.Persons.Add(Person);
            await ctx.SaveChangesAsync(ct);

            return Person.Id;
        }
    }
}
