namespace Market.Application.Modules.Identity.Person.Commands.UpdateRoles;

public class UpdatePersonRolesCommandValidator : AbstractValidator<UpdatePersonRolesCommand>
{
    public UpdatePersonRolesCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
                .WithMessage("A person must be selected.");

        RuleFor(x => x)
            .Must(x => x.IsAdmin || x.IsOrganiser || x.IsUser)
                .WithMessage("At least one role must be assigned.");
    }
}
