using Market.Application.Modules.Events.Events.Commands.Delete;

namespace Market.Tests.EventTests.UnitTests;

public class DeleteEventCommandValidatorTests
{
    private readonly DeleteEventCommandValidator _validator = new();

    [Fact]
    public void Validate_WithPositiveId_Passes()
    {
        Assert.True(_validator.Validate(new DeleteEventCommand { Id = 1 }).IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveId_Fails(int id)
    {
        var result = _validator.Validate(new DeleteEventCommand { Id = id });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.ErrorMessage == "Id must be a positive value.");
    }
}
