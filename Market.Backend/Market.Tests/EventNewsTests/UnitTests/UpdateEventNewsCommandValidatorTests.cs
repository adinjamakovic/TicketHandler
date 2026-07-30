using Market.Application.Modules.Events.EventsNews.Commands.Update;
using Market.Tests.Common;

namespace Market.Tests.EventNewsTests.UnitTests;

public class UpdateEventNewsCommandValidatorTests
{
    private readonly UpdateEventNewsCommandValidator _validator = new();

    private static UpdateEventNewsCommand ValidCommand() => new()
    {
        Id = 1,
        Header = "Doors now open at 18:30",
        Body = "The schedule moved half an hour earlier."
    };

    private static string[] ErrorsFor(UpdateEventNewsCommandValidator validator, UpdateEventNewsCommand command, string property) =>
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
    public void Validate_WithNonPositiveId_Fails(int id)
    {
        var command = ValidCommand();
        command.Id = id;

        Assert.Contains(
            "Id must be a positive value.",
            ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Id)));
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
            ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Header)));
    }

    [Fact]
    public void Validate_WithNullHeader_Fails()
    {
        var command = ValidCommand();
        command.Header = null!;

        Assert.NotEmpty(ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Header)));
    }

    [Fact]
    public void Validate_WithShortHeader_Fails()
    {
        var command = ValidCommand();
        command.Header = "News";

        Assert.Contains(
            "Header must be at least 6 characters.",
            ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Header)));
    }

    [Fact]
    public void Validate_WithHeaderAtMinimumLength_Passes()
    {
        var command = ValidCommand();
        command.Header = new string('a', UpdateEventNewsCommandValidator.MinHeaderLength);

        Assert.Empty(ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Header)));
    }

    [Fact]
    public void Validate_WithHeaderAtMaximumLength_Passes()
    {
        var command = ValidCommand();
        command.Header = new string('a', UpdateEventNewsCommandValidator.MaxHeaderLength);

        Assert.Empty(ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Header)));
    }

    [Fact]
    public void Validate_WithOverlongHeader_Fails()
    {
        var command = ValidCommand();
        command.Header = new string('a', UpdateEventNewsCommandValidator.MaxHeaderLength + 1);

        Assert.Contains(
            "Header must be 50 characters or fewer.",
            ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Header)));
    }

    [Fact]
    public void Validate_WithBodyAtMaximumLength_Passes()
    {
        var command = ValidCommand();
        command.Body = new string('a', UpdateEventNewsCommandValidator.MaxBodyLength);

        Assert.Empty(ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Body)));
    }

    [Fact]
    public void Validate_WithOverlongBody_Fails()
    {
        var command = ValidCommand();
        command.Body = new string('a', UpdateEventNewsCommandValidator.MaxBodyLength + 1);

        Assert.Contains(
            "Body must be 1000 characters or fewer.",
            ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Body)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithMissingBody_Passes(string? body)
    {
        var command = ValidCommand();
        command.Body = body;

        Assert.Empty(ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Body)));
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

        Assert.Empty(ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Image)));
    }

    [Theory]
    [InlineData("news.pdf")]
    [InlineData("news.exe")]
    [InlineData("news")]
    public void Validate_WithDisallowedImageExtension_Fails(string fileName)
    {
        var command = ValidCommand();
        command.Image = new FakeFormFile(fileName);

        Assert.NotEmpty(ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Image)));
    }

    [Fact]
    public void Validate_WithOversizedImage_Fails()
    {
        var command = ValidCommand();
        command.Image = new FakeFormFile("news.png", length: (5 * 1024 * 1024) + 1);

        Assert.Contains(
            "Image must be 5 MB or smaller.",
            ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Image)));
    }

    [Fact]
    public void Validate_WithoutImage_Passes()
    {
        var command = ValidCommand();
        command.Image = null;

        Assert.Empty(ErrorsFor(_validator, command, nameof(UpdateEventNewsCommand.Image)));
    }
}
