using Market.Application.Common.Validation;

namespace Market.Application.Modules.Events.Organizers.Commands.Update;

public class UpdateOrganizerCommandValidator : AbstractValidator<UpdateOrganizerCommand>
{
    public UpdateOrganizerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
                .WithMessage("Id must be a positive value.");

        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Name is required.")
            .MinimumLength(3)
                .WithMessage($"Name must be at least 3 characters.")
            .MaximumLength(50)
                .WithMessage($"Name must be 50 characters or fewer.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
                .WithMessage($"Description must be 500 characters or fewer.");

        RuleFor(x => x.Address)
            .NotEmpty()
                .WithMessage("Address is required.")
            .MaximumLength(200)
                .WithMessage($"Address must be 200 characters or fewer.");

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("A city must be selected.");

        RuleFor(x => x.Logo).ValidImage();

        RuleFor(x => x.User)
            .NotNull().WithMessage("User details are required.")
            .SetValidator(new UpdateOrganizerCommandUserValidator());
    }
}

public class UpdateOrganizerCommandUserValidator : AbstractValidator<UpdateOrganizerCommandUser>
{
    public UpdateOrganizerCommandUserValidator()
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
            .GreaterThan(0).WithMessage("A city must be selected for the user.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("User address is required.")
            .MaximumLength(200)
                .WithMessage($"User address must be 200 characters or fewer.");

        RuleFor(x => x.Gender)
            .NotEmpty()
                .WithMessage("Gender is required.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256)
                .WithMessage($"Email must be 256 characters or fewer.");

        RuleFor(x => x.Password)
            .MinimumLength(8)
                .WithMessage($"Password must be at least 8 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));
    }
}
