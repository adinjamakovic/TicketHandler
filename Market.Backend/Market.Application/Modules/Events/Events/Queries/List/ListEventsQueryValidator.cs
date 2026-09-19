namespace Market.Application.Modules.Events.Events.Queries.List;

public sealed class ListEventsQueryValidator : AbstractValidator<ListEventsQuery>
{
    public ListEventsQueryValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(200).WithMessage("Search term must be 200 characters or fewer.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must be 100 characters or fewer.");

        RuleFor(x => x.EventType)
            .MaximumLength(100).WithMessage("Event type must be 100 characters or fewer.");

        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom!.Value)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage("Date to must be on or after date from.");
    }
}
