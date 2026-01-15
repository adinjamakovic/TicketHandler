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
                    Phone=x.Phone
                })
                .FirstOrDefaultAsync(ct);
            if (Person is null)
                throw new MarketNotFoundException("Person not found");

            return Person;
        }
    }
}
