using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Queries.GetByUserId
{
    public class GetOrganizerByUserIdQueryValidator : AbstractValidator<GetOrganizerByUserIdQuery>
    {
        public GetOrganizerByUserIdQueryValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("Id must be a positive value.");
        }
    }
}
