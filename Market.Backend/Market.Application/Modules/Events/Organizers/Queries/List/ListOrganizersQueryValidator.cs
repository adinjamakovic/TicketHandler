using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Queries.List
{
    public sealed class ListOrganizersQueryValidator : AbstractValidator<ListOrganizersQuery>
    {
        public ListOrganizersQueryValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(200).WithMessage("Search term must be 200 characters or fewer.");

            RuleFor(x => x.City)
                .MaximumLength(100).WithMessage("City must be 100 characters or fewer.");

            RuleFor(x => x.Email)
                .MaximumLength(256).WithMessage("E-mail must be 256 characters or fewer.");
        }
    }
}
