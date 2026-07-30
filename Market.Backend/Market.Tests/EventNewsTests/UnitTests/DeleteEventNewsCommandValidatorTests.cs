using Market.Application.Modules.Events.EventsNews.Commands.Delete;

namespace Market.Tests.EventNewsTests.UnitTests;

public class DeleteEventNewsCommandValidatorTests
{
    private readonly DeleteEventNewsCommandValidator _validator = new();

    [Fact]
    public void Validate_WithPositiveId_Passes()
    {
        Assert.True(_validator.Validate(new DeleteEventNewsCommand { Id = 1 }).IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveId_Fails(int id)
    {
        var result = _validator.Validate(new DeleteEventNewsCommand { Id = id });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.ErrorMessage == "Id must be a positive value.");
    }
}
