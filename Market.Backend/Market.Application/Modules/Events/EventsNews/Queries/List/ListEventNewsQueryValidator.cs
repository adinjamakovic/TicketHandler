using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Queries.List
{
    public class ListEventNewsQueryValidator : AbstractValidator<ListEventNewsQuery>
    {
        public ListEventNewsQueryValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(200).WithMessage("Search term must be 200 characters or fewer.");

            RuleFor(x => x.EventId)
                .GreaterThan(0).WithMessage("Event id must be a positive value.")
                .When(x => x.EventId.HasValue);

            RuleFor(x => x.OrganizerId)
                .GreaterThan(0).WithMessage("Organizer id must be a positive value.")
                .When(x => x.OrganizerId.HasValue);

            RuleFor(x => x.DateTo)
                .GreaterThanOrEqualTo(x => x.DateFrom!.Value)
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
                .WithMessage("Date to must be on or after date from.");
        }
    }
}
