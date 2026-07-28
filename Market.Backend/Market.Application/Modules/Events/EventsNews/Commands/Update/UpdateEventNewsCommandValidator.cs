using Market.Application.Common.Validation;

namespace Market.Application.Modules.Events.EventsNews.Commands.Update;

public class UpdateEventNewsCommandValidator : AbstractValidator<UpdateEventNewsCommand>
{
    public const int MinHeaderLength = 6;
    public const int MaxHeaderLength = 50;
    public const int MaxBodyLength = 1000;

    public UpdateEventNewsCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
                .WithMessage("Id must be a positive value.");

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
