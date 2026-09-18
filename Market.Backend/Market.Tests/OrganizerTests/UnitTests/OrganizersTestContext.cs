using Market.Domain.Entities.Events;
using Market.Domain.Entities.Geographical;
using Market.Tests.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Time.Testing;

namespace Market.Tests.OrganizerTests.UnitTests;

public sealed class OrganizersTestContext : IAsyncDisposable
{
    public const int CountryId = 1;
    public const int SarajevoCityId = 1;
    public const int MostarCityId = 2;
    public const int MissingId = 9999;
    private static readonly InMemoryDatabaseRoot Root = new();
    private readonly DbContextOptions<DatabaseContext> _options;
    private readonly List<DatabaseContext> _contexts = [];
    public DatabaseContext Db { get; }
    public FakeTimeProvider Clock { get; }
    public FakeImageStorage ImageStorage { get; } = new();

    private OrganizersTestContext()
    {
        Clock = new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.Zero));

        _options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase($"organizers-{Guid.NewGuid()}", Root)
            .EnableSensitiveDataLogging()
            .Options;

        Db = NewContext();
    }

    public DatabaseContext NewContext()
    {
        var context = new DatabaseContext(_options, Clock);
        _contexts.Add(context);
        return context;
    }

    /// <summary>Same seeded data, but every SaveChangesAsync blows up.</summary>
    public DatabaseContext NewFailingSaveContext(Exception failure)
    {
        var context = new FailingSaveDatabaseContext(_options, Clock, failure);
        _contexts.Add(context);
        return context;
    }

    public static async Task<OrganizersTestContext> CreateAsync()
    {
        var ctx = new OrganizersTestContext();
        await ctx.SeedAsync();
        return ctx;
    }

    private async Task SeedAsync()
    {
        Db.Countries.Add(new CountryEntity { Id = CountryId, Name = "Bosnia and Herzegovina", IsoCode = "BA", PhoneCode = "+387" });

        Db.Cities.AddRange(
            new CityEntity { Id = SarajevoCityId, CountryId = CountryId, Name = "Sarajevo", PostalCode = "71000" },
            new CityEntity { Id = MostarCityId, CountryId = CountryId, Name = "Mostar", PostalCode = "88000" });

        await Db.SaveChangesAsync(CancellationToken.None);
        Db.ChangeTracker.Clear();
    }

    public async Task<List<OrganizerEntity>> GetOrganizersAsync()
    {
        await using var readContext = NewContext();
        return await readContext.Organizers
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<OrganizerEntity?> GetOrganizerAsync(int id)
    {
        await using var readContext = NewContext();
        return await readContext.Organizers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> CountPersonsAsync()
    {
        await using var readContext = NewContext();
        return await readContext.Persons.AsNoTracking().CountAsync();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var context in _contexts)
            await context.DisposeAsync();
    }

    private sealed class FailingSaveDatabaseContext(
        DbContextOptions<DatabaseContext> options,
        TimeProvider clock,
        Exception failure)
        : DatabaseContext(options, clock)
    {
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            Task.FromException<int>(failure);
    }
}
