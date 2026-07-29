using Market.Domain.Entities.Events;
using Market.Domain.Entities.Geographical;
using Market.Domain.Entities.Identity;
using Market.Tests.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Time.Testing;

namespace Market.Tests.EventTests.UnitTests;

/// <summary>
/// Per-test harness: a fresh in-memory <see cref="DatabaseContext"/> pre-seeded with the
/// geography/organizer/performer graph an event needs, plus the fakes handlers depend on.
/// </summary>
public sealed class EventsTestContext : IAsyncDisposable
{
    public const int CountryId = 1;
    public const int SarajevoCityId = 1;
    public const int MostarCityId = 2;

    public const int OrganizerUserId = 1;
    public const int OtherOrganizerUserId = 2;
    public const int OrganizerId = 1;
    public const int OtherOrganizerId = 2;

    /// <summary>Venue in Sarajevo.</summary>
    public const int VenueId = 1;
    /// <summary>Venue in Mostar.</summary>
    public const int OtherVenueId = 2;

    /// <summary>Event type "Concert".</summary>
    public const int EventTypeId = 1;
    /// <summary>Event type "Theatre".</summary>
    public const int OtherEventTypeId = 2;

    public const int PerformerId = 1;
    public const int OtherPerformerId = 2;

    /// <summary>Id that is guaranteed not to exist in any table.</summary>
    public const int MissingId = 9999;

    /// <summary>
    /// Shared across all tests on purpose — stores stay isolated by database name, while a single
    /// root keeps EF from building a fresh internal service provider per test.
    /// </summary>
    private static readonly InMemoryDatabaseRoot Root = new();

    private readonly DbContextOptions<DatabaseContext> _options;
    private readonly List<DatabaseContext> _contexts = [];

    /// <summary>Context handed to the handler under test.</summary>
    public DatabaseContext Db { get; }

    public FakeTimeProvider Clock { get; }

    public FakeImageStorage ImageStorage { get; } = new();

    /// <summary>A date safely in the future for both the fake clock and the validators' real clock.</summary>
    public DateTime FutureDate => DateTime.UtcNow.AddDays(30);

    private EventsTestContext()
    {
        Clock = new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.Zero));

        _options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase($"events-{Guid.NewGuid()}", Root)
            .EnableSensitiveDataLogging()
            .Options;

        Db = NewContext();
    }

    public static async Task<EventsTestContext> CreateAsync()
    {
        var ctx = new EventsTestContext();
        await ctx.SeedAsync();
        return ctx;
    }

    /// <summary>
    /// A second context over the same store, so assertions read persisted state
    /// instead of the change tracker the handler just mutated.
    /// </summary>
    public DatabaseContext NewContext()
    {
        var context = new DatabaseContext(_options, Clock);
        _contexts.Add(context);
        return context;
    }

    private async Task SeedAsync()
    {
        Db.Countries.Add(new CountryEntity { Id = CountryId, Name = "Bosnia and Herzegovina", PhoneCode = "+387" });

        Db.Cities.AddRange(
            new CityEntity { Id = SarajevoCityId, CountryId = CountryId, Name = "Sarajevo", PostalCode = "71000" },
            new CityEntity { Id = MostarCityId, CountryId = CountryId, Name = "Mostar", PostalCode = "88000" });

        Db.Locations.AddRange(
            new LocationEntity { Id = 1, CityId = SarajevoCityId, Name = "Skenderija", Address = "Terezije bb", Description = "Sports and culture centre" },
            new LocationEntity { Id = 2, CityId = MostarCityId, Name = "Rondo", Address = "Kralja Petra 1", Description = "City square" });

        Db.Venues.AddRange(
            new VenueEntity { Id = VenueId, LocationId = 1, Name = "Mirza Delibasic Hall", Seated = 5000, Standing = 2000 },
            new VenueEntity { Id = OtherVenueId, LocationId = 2, Name = "Mostar Open Air", Seated = 0, Standing = 8000 });

        Db.EventTypes.AddRange(
            new EventTypeEntity { Id = EventTypeId, Name = "Concert", IsEnabled = true },
            new EventTypeEntity { Id = OtherEventTypeId, Name = "Theatre", IsEnabled = true });

        Db.Genres.Add(new GenreEntity { Id = 1, Name = "Rock", Description = "Rock music" });

        Db.Performers.AddRange(
            new PerformerEntity { Id = PerformerId, GenreId = 1, Name = "Bijelo Dugme", Description = "Headliner" },
            new PerformerEntity { Id = OtherPerformerId, GenreId = 1, Name = "Zabranjeno Pusenje", Description = "Support act" });

        Db.Persons.AddRange(
            NewPerson(OrganizerUserId, "organiser.one"),
            NewPerson(OtherOrganizerUserId, "organiser.two"));

        Db.Organizers.AddRange(
            new OrganizerEntity
            {
                Id = OrganizerId,
                UserId = OrganizerUserId,
                CityId = SarajevoCityId,
                Name = "Sarajevo Events",
                Description = "Primary organizer",
                Address = "Marsala Tita 1"
            },
            new OrganizerEntity
            {
                Id = OtherOrganizerId,
                UserId = OtherOrganizerUserId,
                CityId = MostarCityId,
                Name = "Mostar Events",
                Description = "Competing organizer",
                Address = "Rade Bitange 5"
            });

        await Db.SaveChangesAsync(CancellationToken.None);
        Db.ChangeTracker.Clear();
    }

    private static PersonEntity NewPerson(int id, string userName) => new()
    {
        Id = id,
        CityId = SarajevoCityId,
        FirstName = "Test",
        LastName = "Organiser",
        BirthDate = new DateTime(1990, 1, 1),
        UserName = userName,
        Email = $"{userName}@test.local",
        PasswordHash = "hash",
        IsOrganiser = true,
        IsEnabled = true
    };

    /// <summary>
    /// Persists an event (and optional performer line-ups) directly, bypassing the handlers.
    /// </summary>
    public async Task<EventEntity> AddEventAsync(
        string name = "Seeded Event",
        int organizerId = OrganizerId,
        int venueId = VenueId,
        int eventTypeId = EventTypeId,
        DateTime? scheduledDate = null,
        string? image = null,
        params (int PerformerId, TimeOnly TimeStamp)[] performers)
    {
        await using var seedContext = NewContext();

        var entity = new EventEntity
        {
            Name = name,
            Description = $"{name} description",
            ScheduledDate = scheduledDate ?? Clock.GetUtcNow().UtcDateTime.AddDays(30),
            OrganizerId = organizerId,
            VenueId = venueId,
            EventTypeId = eventTypeId,
            Image = image
        };

        seedContext.Events.Add(entity);
        await seedContext.SaveChangesAsync(CancellationToken.None);

        foreach (var (performerId, timeStamp) in performers)
        {
            seedContext.PerformerEvents.Add(new PerformerEventEntity
            {
                EventId = entity.Id,
                PerformerId = performerId,
                TimeStamp = timeStamp
            });
        }

        if (performers.Length > 0)
            await seedContext.SaveChangesAsync(CancellationToken.None);

        return entity;
    }

    /// <summary>Performer line-up rows for an event, ordered by id, read from a fresh context.</summary>
    public async Task<List<PerformerEventEntity>> GetPerformerEventsAsync(int eventId)
    {
        await using var readContext = NewContext();
        return await readContext.PerformerEvents
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var context in _contexts)
            await context.DisposeAsync();
    }
}
