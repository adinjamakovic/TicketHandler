namespace Market.Application.Modules.Events.Organizers.Commands.Delete;

public class DeleteOrganizerCommandValidator : AbstractValidator<DeleteOrganizerCommand>
{
    public DeleteOrganizerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be a positive value.");
    }
}
