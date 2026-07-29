using Market.Application.Modules.Events.Events.Commands.Update;
using Market.Tests.Common;

namespace Market.Tests.EventTests.UnitTests;

public class UpdateEventCommandValidatorTests
{
    private readonly UpdateEventCommandValidator _validator = new();

    private static UpdateEventCommand ValidCommand() => new()
    {
        Id = 1,
        Name = "Rock Night Sarajevo",
        Description = "Open air concert",
        ScheduledDate = DateTime.UtcNow.AddDays(30),
        VenueId = 1,
        EventTypeId = 1
    };

    private static string[] ErrorsFor(UpdateEventCommandValidator validator, UpdateEventCommand command, string property) =>
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
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_WithNonPositiveId_Fails(int id)
    {
        var command = ValidCommand();
        command.Id = id;

        Assert.Contains("Id must be a positive value.", ErrorsFor(_validator, command, nameof(UpdateEventCommand.Id)));
    }

    [Fact]
    public void Validate_WithShortName_Fails()
    {
        var command = ValidCommand();
        command.Name = "Gig";

        Assert.Contains("Name must be at least 5 characters.", ErrorsFor(_validator, command, nameof(UpdateEventCommand.Name)));
    }

    [Fact]
    public void Validate_WithOverlongName_Fails()
    {
        var command = ValidCommand();
        command.Name = new string('a', 101);

        Assert.Contains("Name must be 100 characters or fewer.", ErrorsFor(_validator, command, nameof(UpdateEventCommand.Name)));
    }

    [Fact]
    public void Validate_WithOverlongDescription_Fails()
    {
        var command = ValidCommand();
        command.Description = new string('a', 2001);

        Assert.Contains("Description must be 2000 characters or fewer.", ErrorsFor(_validator, command, nameof(UpdateEventCommand.Description)));
    }

    /// <summary>
    /// Unlike create, an update may legitimately touch an event that has already happened.
    /// </summary>
    [Fact]
    public void Validate_WithPastScheduledDate_Passes()
    {
        var command = ValidCommand();
        command.ScheduledDate = DateTime.UtcNow.AddYears(-1);

        Assert.Empty(ErrorsFor(_validator, command, nameof(UpdateEventCommand.ScheduledDate)));
    }

    [Fact]
    public void Validate_WithDefaultScheduledDate_Fails()
    {
        var command = ValidCommand();
        command.ScheduledDate = default;

        Assert.Contains("Scheduled date is required.", ErrorsFor(_validator, command, nameof(UpdateEventCommand.ScheduledDate)));
    }

    [Fact]
    public void Validate_WithNonPositiveVenueId_Fails()
    {
        var command = ValidCommand();
        command.VenueId = 0;

        Assert.Contains("A venue must be selected.", ErrorsFor(_validator, command, nameof(UpdateEventCommand.VenueId)));
    }

    [Fact]
    public void Validate_WithNonPositiveEventTypeId_Fails()
    {
        var command = ValidCommand();
        command.EventTypeId = 0;

        Assert.Contains("An event type must be selected.", ErrorsFor(_validator, command, nameof(UpdateEventCommand.EventTypeId)));
    }

    [Fact]
    public void Validate_WithDisallowedImageExtension_Fails()
    {
        var command = ValidCommand();
        command.Image = new FakeFormFile("poster.pdf");

        Assert.NotEmpty(ErrorsFor(_validator, command, nameof(UpdateEventCommand.Image)));
    }

    [Fact]
    public void Validate_WithOversizedImage_Fails()
    {
        var command = ValidCommand();
        command.Image = new FakeFormFile("poster.png", length: (5 * 1024 * 1024) + 1);

        Assert.Contains("Image must be 5 MB or smaller.", ErrorsFor(_validator, command, nameof(UpdateEventCommand.Image)));
    }

    [Fact]
    public void Validate_WithDuplicatePerformer_Fails()
    {
        var command = ValidCommand();
        command.Performers =
        [
            new UpdateEventCommandPerformers { Id = 0, PerformerId = 1, TimeStamp = new TimeOnly(20, 0) },
            new UpdateEventCommandPerformers { Id = 0, PerformerId = 1, TimeStamp = new TimeOnly(22, 0) }
        ];

        Assert.Contains(
            "The same performer cannot be added to an event twice.",
            ErrorsFor(_validator, command, nameof(UpdateEventCommand.Performers)));
    }

    [Fact]
    public void Validate_WithNewAndExistingPerformerEntries_Passes()
    {
        var command = ValidCommand();
        command.Performers =
        [
            new UpdateEventCommandPerformers { Id = 0, PerformerId = 1, TimeStamp = new TimeOnly(20, 0) },
            new UpdateEventCommandPerformers { Id = 7, PerformerId = 2, TimeStamp = new TimeOnly(22, 0) }
        ];

        var result = _validator.Validate(command);

        Assert.True(result.IsValid, string.Join(" | ", result.Errors.Select(x => x.ErrorMessage)));
    }

    [Fact]
    public void Validate_WithNegativePerformerEntryId_Fails()
    {
        var command = ValidCommand();
        command.Performers =
        [
            new UpdateEventCommandPerformers { Id = -1, PerformerId = 1, TimeStamp = new TimeOnly(20, 0) }
        ];

        var result = _validator.Validate(command);

        Assert.Contains(result.Errors, x => x.ErrorMessage == "Performer entry id must be 0 (new) or a positive value.");
    }

    [Fact]
    public void Validate_WithNonPositivePerformerId_Fails()
    {
        var command = ValidCommand();
        command.Performers =
        [
            new UpdateEventCommandPerformers { Id = 0, PerformerId = 0, TimeStamp = new TimeOnly(20, 0) }
        ];

        var result = _validator.Validate(command);

        Assert.Contains(result.Errors, x => x.ErrorMessage == "A performer must be selected.");
    }
}
