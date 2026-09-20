using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Identity.Person.Queries.GetById
{
    public class GetPersonByIdQueryHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
        : IRequestHandler<GetPersonByIdQuery, GetPersonByIdQueryDto>
    {
        public async Task<GetPersonByIdQueryDto> Handle(GetPersonByIdQuery req, CancellationToken ct)
        {
            // The DTO carries email, address, phone and birth date, so only the person themselves
            // may read it. Admins are allowed through because they administer organizer accounts.
            if (!appCurrentUser.IsAdmin && appCurrentUser.UserId != req.Id)
                throw new MarketBusinessRuleException("111", "This user can not view this person");

            var Person = await ctx.Persons
                .Where(x => x.Id == req.Id)
                .Select(x => new GetPersonByIdQueryDto
                {
                    FirstName=x.FirstName,
                    LastName=x.LastName,
                    BirthDate=x.BirthDate,
                    CityId=x.CityId,
                    Address=x.Address,
                    Gender=x.Gender,
                    Phone=x.Phone,
                    Email=x.Email
                })
                .FirstOrDefaultAsync(ct);
            if (Person is null)
                throw new MarketNotFoundException("Person not found");

            return Person;
        }
    }
}
