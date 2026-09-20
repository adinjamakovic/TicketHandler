using FluentValidation;
using Market.Application.Abstractions;
using Market.Application.Common.Exceptions;
using Market.Application.Modules.Events.Organizers.Commands.Create;
using Market.Tests.Common;

namespace Market.Tests.OrganizerTests.UnitTests;

public class CreateOrganizerCommandHandlerTests
{
    private static CreateOrganizerCommandHandler CreateHandler(
        OrganizersTestContext ctx,
        FakeAppCurrentUser currentUser,
        IAppDbContext? db = null) =>
        new(db ?? ctx.Db, currentUser, ctx.ImageStorage);

    private static CreateOrganizerCommand ValidCommand() => new()
    {
        Name = "Sarajevo Events",
        Description = "Primary organizer",
        Address = "Marsala Tita 1",
        CityId = OrganizersTestContext.SarajevoCityId,
        Logo = null,
        User = new CreateOrganizerCommandUser
        {
            FirstName = "Test",
            LastName = "Organiser",
            BirthDate = new DateTime(1990, 1, 1),
            CityId = OrganizersTestContext.SarajevoCityId,
            Address = "Marsala Tita 1",
            Gender = "M",
            Phone = "+38761123456",
            Email = "organiser@test.local",
            Password = "Password123!"
        }
    };

    [Fact]
    public async Task Handle_WhenCallerIsNotAdmin_ThrowsBusinessRule()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Organiser(1));

        var ex = await Assert.ThrowsAsync<MarketBusinessRuleException>(
            () => handler.Handle(ValidCommand(), CancellationToken.None));

        Assert.Equal("Only an admin can add an organizer", ex.Message);
        Assert.Empty(await ctx.GetOrganizersAsync());
    }

    [Fact]
    public async Task Handle_WhenCityDoesNotExist_ThrowsValidation()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        var command = ValidCommand();
        command.CityId = OrganizersTestContext.MissingId;

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Empty(ctx.ImageStorage.Saved);
    }

    [Fact]
    public async Task Handle_WithValidCommand_PersistsOrganizerAndItsUser()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        ctx.ImageStorage.SavedPath = "organizers/logo.png";

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());
        var command = ValidCommand();
        command.Logo = new FakeFormFile("logo.png");

        var id = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(0, id);

        var created = await ctx.GetOrganizerAsync(id);

        Assert.NotNull(created);
        Assert.Equal("Sarajevo Events", created!.Name);
        Assert.Equal("organizers/logo.png", created.Logo);
        Assert.False(created.IsDeleted);
        Assert.Equal(1, await ctx.CountPersonsAsync());
        Assert.Empty(ctx.ImageStorage.Deleted);
    }

    [Fact]
    public async Task Handle_WhenNameAlreadyExists_ThrowsConflict()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin());

        await handler.Handle(ValidCommand(), CancellationToken.None);

        var duplicate = ValidCommand();
        duplicate.Name = "  Sarajevo Events  ";

        await Assert.ThrowsAsync<MarketConflictException>(
            () => handler.Handle(duplicate, CancellationToken.None));

        Assert.Single(await ctx.GetOrganizersAsync());
    }

    [Fact]
    public async Task Handle_WhenSaveFails_RethrowsInsteadOfReportingSuccess()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        var failure = new DbUpdateException("save failed");
        var failingDb = ctx.NewFailingSaveContext(failure);

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin(), failingDb);

        var thrown = await Assert.ThrowsAsync<DbUpdateException>(
            () => handler.Handle(ValidCommand(), CancellationToken.None));

        Assert.Same(failure, thrown);
        Assert.Empty(await ctx.GetOrganizersAsync());
    }

    [Fact]
    public async Task Handle_WhenSaveFails_DeletesTheUploadedLogo()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        ctx.ImageStorage.SavedPath = "organizers/logo.png";
        var failingDb = ctx.NewFailingSaveContext(new DbUpdateException("save failed"));

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin(), failingDb);
        var command = ValidCommand();
        command.Logo = new FakeFormFile("logo.png");

        await Assert.ThrowsAsync<DbUpdateException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("organizers/logo.png", Assert.Single(ctx.ImageStorage.Deleted));
    }

    [Fact]
    public async Task Handle_WhenSaveIsCancelled_StillDeletesTheUploadedLogo()
    {
        await using var ctx = await OrganizersTestContext.CreateAsync();
        ctx.ImageStorage.SavedPath = "organizers/logo.png";
        var failingDb = ctx.NewFailingSaveContext(new OperationCanceledException());

        var handler = CreateHandler(ctx, FakeAppCurrentUser.Admin(), failingDb);
        var command = ValidCommand();
        command.Logo = new FakeFormFile("logo.png");

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal("organizers/logo.png", Assert.Single(ctx.ImageStorage.Deleted));
    }
}
