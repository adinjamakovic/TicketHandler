using Market.Application.Common.Validation;

namespace Market.Application.Modules.Events.Events.Commands.Update;

public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
                .WithMessage("Id must be a positive value.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(5)
                .WithMessage($"Name must be at least 5 characters.")
            .MaximumLength(100)
                .WithMessage($"Name must be 100 characters or fewer.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
                .WithMessage($"Description must be 2000 characters or fewer.");

        // No future-date rule here. A date that is already scheduled may need edits after it has happened.
        RuleFor(x => x.ScheduledDate)
            .NotEmpty()
                .WithMessage("Scheduled date is required.");

        RuleFor(x => x.VenueId)
            .GreaterThan(0)
                .WithMessage("A venue must be selected.");

        RuleFor(x => x.EventTypeId)
            .GreaterThan(0)
                .WithMessage("An event type must be selected.");

        RuleFor(x => x.Image).ValidImage();

        RuleFor(x => x.Performers)
            .NotNull().WithMessage("Performers must be provided.")
            .Must(performers => performers.Select(p => p.PerformerId).Distinct().Count() == performers.Count)
                .WithMessage("The same performer cannot be added to an event twice.");

        RuleForEach(x => x.Performers).ChildRules(performer =>
        {
            performer.RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                    .WithMessage("Performer entry id must be 0 (new) or a positive value.");

            performer.RuleFor(x => x.PerformerId)
                .GreaterThan(0).WithMessage("A performer must be selected.");
        });
    }
}
