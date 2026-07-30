namespace Market.Application.Modules.Identity.Person.Commands.Create;

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
                .WithMessage("First name is required.")
            .MaximumLength(100)
                .WithMessage($"First name must be 100 characters or fewer.");

        RuleFor(x => x.LastName)
            .NotEmpty()
                .WithMessage("Last name is required.")
            .MaximumLength(100)
                .WithMessage($"Last name must be 100 characters or fewer.");

        RuleFor(x => x.BirthDate)
            .NotEmpty()
                .WithMessage("Birth date is required.")
            .GreaterThanOrEqualTo(new DateTime(1900, 1, 1))
                .WithMessage("Birth date is not a valid date.")
            .LessThan(_ => DateTime.UtcNow)
                .WithMessage("Birth date must be in the past.");

        RuleFor(x => x.CityId)
            .GreaterThan(0)
                .WithMessage("A city must be selected.");

        RuleFor(x => x.Address)
            .NotEmpty()
                .WithMessage("Address is required.")
            .MaximumLength(200)
                .WithMessage($"Address must be 200 characters or fewer.");

        RuleFor(x => x.Gender)
            .NotEmpty()
                .WithMessage("Gender is required.");

        RuleFor(x => x.Phone)
            .NotEmpty()
                .WithMessage("Phone is required.");

        RuleFor(x => x.Username)
            .NotEmpty()
                .WithMessage("Username is required.")
            .MinimumLength(3)
                .WithMessage($"Username must be at least 3 characters.")
            .MaximumLength(100)
                .WithMessage($"Username must be 100 characters or fewer.");

        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Email is required.")
            .EmailAddress()
                .WithMessage("A valid email address is required.")
            .MaximumLength(200)
                .WithMessage($"Email must be 200 characters or fewer.");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("Password is required.")
            .MinimumLength(8)
                .WithMessage($"Password must be at least 8 characters.");
    }
}
