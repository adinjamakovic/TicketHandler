using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Queries.GetById
{
    public class GetOrganizerByIdQueryValidator : AbstractValidator<GetOrganizerByIdQuery>
    {
        public GetOrganizerByIdQueryValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be a positive value.");
        }
    }
}
