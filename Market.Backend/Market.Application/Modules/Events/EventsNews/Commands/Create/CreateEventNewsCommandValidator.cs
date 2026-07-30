using Market.Application.Common.Validation;

namespace Market.Application.Modules.Events.EventsNews.Commands.Create;

public class CreateEventNewsCommandValidator : AbstractValidator<CreateEventNewsCommand>
{
    public CreateEventNewsCommandValidator()
    {
        RuleFor(x => x.EventId)
            .GreaterThan(0).WithMessage("An event must be selected.");

        RuleFor(x => x.Header)
            .NotEmpty().WithMessage("Header is required.")
            .MinimumLength(6)
                .WithMessage($"Header must be at least 6 characters.")
            .MaximumLength(50)
                .WithMessage($"Header must be 50 characters or fewer.");

        RuleFor(x => x.Body)
            .MaximumLength(1000)
                .WithMessage($"Body must be 1000 characters or fewer.");

        RuleFor(x => x.Image).ValidImage();
    }
}
