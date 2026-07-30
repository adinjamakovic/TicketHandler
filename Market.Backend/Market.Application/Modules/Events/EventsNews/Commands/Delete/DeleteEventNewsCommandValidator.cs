namespace Market.Application.Modules.Events.EventsNews.Commands.Delete;

public class DeleteEventNewsCommandValidator : AbstractValidator<DeleteEventNewsCommand>
{
    public DeleteEventNewsCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be a positive value.");
    }
}
