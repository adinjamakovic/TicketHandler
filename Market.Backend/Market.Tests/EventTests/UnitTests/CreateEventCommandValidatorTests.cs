using Market.Application.Modules.Events.Events.Commands.Create;
using Market.Tests.Common;

namespace Market.Tests.EventTests.UnitTests;

public class CreateEventCommandValidatorTests
{
    private readonly CreateEventCommandValidator _validator = new();

    private static CreateEventCommand ValidCommand() => new()
    {
        Name = "Rock Night Sarajevo",
        Description = "Open air concert",
        ScheduledDate = DateTime.UtcNow.AddDays(30),
        VenueId = 1,
        EventTypeId = 1
    };

    private static string[] ErrorsFor(CreateEventCommandValidator validator, CreateEventCommand command, string property) =>
        validator.Validate(command).Errors
            .Where(x => x.PropertyName == property)
            .Select(x => x.ErrorMessage)
            .ToArray();

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var result = _validator.Validate(ValidCommand());

        Assert.True(result.IsValid, string.Join(" | ", result.Errors.Select(x => x.ErrorMessage)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithMissingName_Fails(string name)
    {
        var command = ValidCommand();
        command.Name = name;

        Assert.NotEmpty(ErrorsFor(_validator, command, nameof(CreateEventCommand.Name)));
    }

    [Fact]
    public void Validate_WithShortName_Fails()
    {
        var command = ValidCommand();
        command.Name = "Gig";

        Assert.Contains("Name must be at least 5 characters.", ErrorsFor(_validator, command, nameof(CreateEventCommand.Name)));
    }

    [Fact]
    public void Validate_WithOverlongName_Fails()
    {
        var command = ValidCommand();
        command.Name = new string('a', 101);

        Assert.Contains("Name must be 100 characters or fewer.", ErrorsFor(_validator, command, nameof(CreateEventCommand.Name)));
    }

    [Fact]
    public void Validate_WithOverlongDescription_Fails()
    {
        var command = ValidCommand();
        command.Description = new string('a', 2001);

        Assert.Contains("Description must be 2000 characters or fewer.", ErrorsFor(_validator, command, nameof(CreateEventCommand.Description)));
    }

    [Fact]
    public void Validate_WithNullDescription_Passes()
    {
        var command = ValidCommand();
        command.Description = null;

        Assert.Empty(ErrorsFor(_validator, command, nameof(CreateEventCommand.Description)));
    }

    [Fact]
    public void Validate_WithPastScheduledDate_Fails()
    {
        var command = ValidCommand();
        command.ScheduledDate = DateTime.UtcNow.AddDays(-1);

        Assert.Contains("Scheduled date must be in the future.", ErrorsFor(_validator, command, nameof(CreateEventCommand.ScheduledDate)));
    }

    [Fact]
    public void Validate_WithDefaultScheduledDate_Fails()
    {
        var command = ValidCommand();
        command.ScheduledDate = default;

        Assert.NotEmpty(ErrorsFor(_validator, command, nameof(CreateEventCommand.ScheduledDate)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveVenueId_Fails(int venueId)
    {
        var command = ValidCommand();
        command.VenueId = venueId;

        Assert.Contains("A venue must be selected.", ErrorsFor(_validator, command, nameof(CreateEventCommand.VenueId)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveEventTypeId_Fails(int eventTypeId)
    {
        var command = ValidCommand();
        command.EventTypeId = eventTypeId;

        Assert.Contains("An event type must be selected.", ErrorsFor(_validator, command, nameof(CreateEventCommand.EventTypeId)));
    }

    [Theory]
    [InlineData("poster.jpg")]
    [InlineData("poster.JPEG")]
    [InlineData("poster.png")]
    [InlineData("poster.webp")]
    [InlineData("poster.gif")]
    public void Validate_WithAllowedImageExtension_Passes(string fileName)
    {
        var command = ValidCommand();
        command.Image = new FakeFormFile(fileName);

        Assert.Empty(ErrorsFor(_validator, command, nameof(CreateEventCommand.Image)));
    }

    [Theory]
    [InlineData("poster.pdf")]
    [InlineData("poster.exe")]
    [InlineData("poster")]
    public void Validate_WithDisallowedImageExtension_Fails(string fileName)
    {
        var command = ValidCommand();
        command.Image = new FakeFormFile(fileName);

        Assert.NotEmpty(ErrorsFor(_validator, command, nameof(CreateEventCommand.Image)));
    }

    [Fact]
    public void Validate_WithOversizedImage_Fails()
    {
        var command = ValidCommand();
        command.Image = new FakeFormFile("poster.png", length: (5 * 1024 * 1024) + 1);

        Assert.Contains("Image must be 5 MB or smaller.", ErrorsFor(_validator, command, nameof(CreateEventCommand.Image)));
    }

    [Fact]
    public void Validate_WithoutImage_Passes()
    {
        var command = ValidCommand();
        command.Image = null;

        Assert.Empty(ErrorsFor(_validator, command, nameof(CreateEventCommand.Image)));
    }

    [Fact]
    public void Validate_WithDuplicatePerformer_Fails()
    {
        var command = ValidCommand();
        command.Performers =
        [
            new CreateEventCommandPerformer { PerformerId = 1, TimeStamp = new TimeOnly(20, 0) },
            new CreateEventCommandPerformer { PerformerId = 1, TimeStamp = new TimeOnly(22, 0) }
        ];

        Assert.Contains(
            "The same performer cannot be added to an event twice.",
            ErrorsFor(_validator, command, nameof(CreateEventCommand.Performers)));
    }

    [Fact]
    public void Validate_WithDistinctPerformers_Passes()
    {
        var command = ValidCommand();
        command.Performers =
        [
            new CreateEventCommandPerformer { PerformerId = 1, TimeStamp = new TimeOnly(20, 0) },
            new CreateEventCommandPerformer { PerformerId = 2, TimeStamp = new TimeOnly(22, 0) }
        ];

        Assert.True(_validator.Validate(command).IsValid);
    }

    [Fact]
    public void Validate_WithNonPositivePerformerId_Fails()
    {
        var command = ValidCommand();
        command.Performers =
        [
            new CreateEventCommandPerformer { PerformerId = 0, TimeStamp = new TimeOnly(20, 0) }
        ];

        var result = _validator.Validate(command);

        Assert.Contains(result.Errors, x => x.ErrorMessage == "A performer must be selected.");
    }
}
