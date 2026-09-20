using Market.Application.Common.Exceptions;
using Market.Application.Modules.Identity.Person.Queries.GetById;
using Market.Tests.Common;

namespace Market.Tests.PersonTests.UnitTests;

public class GetPersonByIdQueryHandlerTests
{
    private static GetPersonByIdQueryHandler CreateHandler(PersonTestContext ctx, FakeAppCurrentUser currentUser) =>
        new(ctx.Db, currentUser);

    [Fact]
    public async Task Handle_WhenReadingAnotherPerson_ThrowsBusinessRule()
    {
        await using var ctx = await PersonTestContext.CreateAsync();

        // A signed-in user asking for somebody else's contact details.
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(PersonTestContext.OwnerPersonId));

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(
                new GetPersonByIdQuery { Id = PersonTestContext.OtherPersonId },
                CancellationToken.None));

        Assert.Equal("This user can not view this person", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenOrganiserReadsAnotherPerson_ThrowsBusinessRule()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(PersonTestContext.OwnerPersonId));

        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(
                new GetPersonByIdQuery { Id = PersonTestContext.OtherPersonId },
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenPersonIsMissingAndNotTheCaller_ThrowsBusinessRuleBeforeTouchingTheDatabase()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(PersonTestContext.OwnerPersonId));

        // Probing for ids must not be able to tell existing persons from missing ones.
        await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(
                new GetPersonByIdQuery { Id = PersonTestContext.MissingId },
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenReadingOwnRecord_ReturnsTheContactDetails()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.User(PersonTestContext.OwnerPersonId));

        var dto = await handler.Handle(
            new GetPersonByIdQuery { Id = PersonTestContext.OwnerPersonId },
            CancellationToken.None);

        Assert.Equal("owner@test.local", dto.Email);
        Assert.Equal("+38761111111", dto.Phone);
        Assert.Equal("Marsala Tita 1", dto.Address);
    }

    [Fact]
    public async Task Handle_WhenCallerIsAdmin_ReturnsAnyPerson()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        var dto = await handler.Handle(
            new GetPersonByIdQuery { Id = PersonTestContext.OtherPersonId },
            CancellationToken.None);

        Assert.Equal("victim@test.local", dto.Email);
    }

    [Fact]
    public async Task Handle_WhenAdminAsksForAMissingPerson_ThrowsNotFound()
    {
        await using var ctx = await PersonTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        await Assert.ThrowsAsync<MarketNotFoundException>(
            () => handler.Handle(
                new GetPersonByIdQuery { Id = PersonTestContext.MissingId },
                CancellationToken.None));
    }
}
