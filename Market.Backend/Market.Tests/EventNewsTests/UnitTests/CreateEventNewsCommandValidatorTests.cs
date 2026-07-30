using Market.Application.Modules.Events.EventsNews.Commands.Create;
using Market.Tests.Common;

namespace Market.Tests.EventNewsTests.UnitTests;

public class CreateEventNewsCommandValidatorTests
{
    private readonly CreateEventNewsCommandValidator _validator = new();

    private static CreateEventNewsCommand ValidCommand() => new()
    {
        EventId = 1,
        Header = "Doors open at 19:00",
        Body = "Gates open one hour before the first act."
    };

    private static string[] ErrorsFor(CreateEventNewsCommandValidator validator, CreateEventNewsCommand command, string property) =>
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
    [InlineData(-1)]
    public void Validate_WithNonPositiveEventId_Fails(int eventId)
    {
        var command = ValidCommand();
        command.EventId = eventId;

        Assert.Contains(
            "An event must be selected.",
            ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.EventId)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithMissingHeader_Fails(string header)
    {
        var command = ValidCommand();
        command.Header = header;

        Assert.Contains(
            "Header is required.",
            ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.Header)));
    }

    [Fact]
    public void Validate_WithShortHeader_Fails()
    {
        var command = ValidCommand();
        command.Header = "News";

        Assert.Contains(
            "Header must be at least 6 characters.",
            ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.Header)));
    }

    [Fact]
    public void Validate_WithHeaderAtMinimumLength_Passes()
    {
        var command = ValidCommand();
        command.Header = new string('a', 6);

        Assert.Empty(ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.Header)));
    }

    [Fact]
    public void Validate_WithHeaderAtMaximumLength_Passes()
    {
        var command = ValidCommand();
        command.Header = new string('a', 50);

        Assert.Empty(ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.Header)));
    }

    [Fact]
    public void Validate_WithOverlongHeader_Fails()
    {
        var command = ValidCommand();
        command.Header = new string('a', 51);

        Assert.Contains(
            "Header must be 50 characters or fewer.",
            ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.Header)));
    }

    [Fact]
    public void Validate_WithBodyAtMaximumLength_Passes()
    {
        var command = ValidCommand();
        command.Body = new string('a', 1000);

        Assert.Empty(ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.Body)));
    }

    [Fact]
    public void Validate_WithOverlongBody_Fails()
    {
        var command = ValidCommand();
        command.Body = new string('a', 1001);

        Assert.Contains(
            "Body must be 1000 characters or fewer.",
            ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.Body)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithMissingBody_Passes(string? body)
    {
        var command = ValidCommand();
        command.Body = body;

        Assert.Empty(ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.Body)));
    }

    [Theory]
    [InlineData("news.jpg")]
    [InlineData("news.JPEG")]
    [InlineData("news.png")]
    [InlineData("news.webp")]
    [InlineData("news.gif")]
    public void Validate_WithAllowedImageExtension_Passes(string fileName)
    {
        var command = ValidCommand();
        command.Image = new FakeFormFile(fileName);

        Assert.Empty(ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.Image)));
    }

    [Theory]
    [InlineData("news.pdf")]
    [InlineData("news.exe")]
    [InlineData("news")]
    public void Validate_WithDisallowedImageExtension_Fails(string fileName)
    {
        var command = ValidCommand();
        command.Image = new FakeFormFile(fileName);

        Assert.NotEmpty(ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.Image)));
    }

    [Fact]
    public void Validate_WithOversizedImage_Fails()
    {
        var command = ValidCommand();
        command.Image = new FakeFormFile("news.png", length: (5 * 1024 * 1024) + 1);

        Assert.Contains(
            "Image must be 5 MB or smaller.",
            ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.Image)));
    }

    [Fact]
    public void Validate_WithoutImage_Passes()
    {
        var command = ValidCommand();
        command.Image = null;

        Assert.Empty(ErrorsFor(_validator, command, nameof(CreateEventNewsCommand.Image)));
    }
}
