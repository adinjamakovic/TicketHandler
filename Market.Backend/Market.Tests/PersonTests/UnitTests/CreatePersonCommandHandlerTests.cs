using FluentValidation;
using Market.Application.Common.Exceptions;
using Market.Application.Modules.Identity.Person.Commands.Create;

namespace Market.Tests.PersonTests.UnitTests;

public class CreatePersonCommandHandlerTests
{
    private static CreatePersonCommand NewCommand(string email = "newcomer@test.local") => new()
    {
        FirstName = "New",
        LastName = "Comer",
        BirthDate = new DateTime(1995, 5, 5),
        CityId = PersonTestContext.SarajevoCityId,
        Address = "Titova 10",
        Gender = "M",
        Phone = "+38763333333",
        Username = "newcomer",
        Email = email,
        Password = "Str0ng!Password"
    };

    [Fact]
    public async Task Handle_WhenRegistering_CreatesAPlainUserWithoutPrivilegedRoles()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = new CreatePersonCommandHandler(ctx.Db);

        var id = await handler.Handle(NewCommand(), CancellationToken.None);

        var person = await ctx.NewContext().Persons.FirstAsync(x => x.Id == id);

        // Registration is a public flow, so the roles come from the server, never the request.
        Assert.False(person.IsAdmin);
        Assert.False(person.IsOrganiser);
        Assert.True(person.IsUser);
        Assert.True(person.IsEnabled);
    }

    [Fact]
    public async Task Handle_WhenTheEmailOnlyDiffersInCasing_ThrowsConflict()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = new CreatePersonCommandHandler(ctx.Db);

        // Login matches the email case-insensitively, so this would give one login two accounts.
        await Assert.ThrowsAsync<MarketConflictException>(
            () => handler.Handle(NewCommand("Owner@Test.Local"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenTheCityDoesNotExist_ThrowsValidation()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = new CreatePersonCommandHandler(ctx.Db);

        var command = NewCommand();
        command.CityId = PersonTestContext.MissingId;

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenTheEmailIsPadded_StoresItTrimmed()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = new CreatePersonCommandHandler(ctx.Db);

        var id = await handler.Handle(NewCommand("  spaced@test.local  "), CancellationToken.None);

        var person = await ctx.NewContext().Persons.FirstAsync(x => x.Id == id);

        Assert.Equal("spaced@test.local", person.Email);
    }
}
