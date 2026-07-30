using Market.Domain.Entities.Events;
using Market.Domain.Entities.Geographical;
using Market.Domain.Entities.Identity;
using Market.Tests.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Time.Testing;

namespace Market.Tests.EventNewsTests.UnitTests;
public sealed class EventNewsTestContext : IAsyncDisposable
{
    public const int CountryId = 1;
    public const int SarajevoCityId = 1;
    public const int MostarCityId = 2;
    public const int OrganizerUserId = 1;
    public const int OtherOrganizerUserId = 2;
    public const int OrganizerId = 1;
    public const int OtherOrganizerId = 2;
    public const int VenueId = 1;
    public const int OtherVenueId = 2;
    public const int EventTypeId = 1;
    public const int OtherEventTypeId = 2;
    public const int RockNightEventId = 1;
    public const int SummerFestivalEventId = 2;
    public const int RockNightNewsId = 1;
    public const int RockNightSecondNewsId = 2;
    public const int SummerFestivalNewsId = 3;
    public const int MissingId = 9999;
    public const string SeededNewsImage = "event-news/seeded-news.png";
    private static readonly InMemoryDatabaseRoot Root = new();
    private readonly DbContextOptions<DatabaseContext> _options;
    private readonly List<DatabaseContext> _contexts = [];
    public DatabaseContext Db { get; }
    public FakeTimeProvider Clock { get; }
    public FakeImageStorage ImageStorage { get; } = new();

    private EventNewsTestContext()
    {
        Clock = new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.Zero));

        _options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase($"event-news-{Guid.NewGuid()}", Root)
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
    public static async Task<EventNewsTestContext> CreateAsync()
    {
        var ctx = new EventNewsTestContext();
        await ctx.SeedAsync();
        return ctx;
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
            new EventTypeEntity { Id = OtherEventTypeId, Name = "Festival", IsEnabled = true });

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

        Db.Events.AddRange(
            new EventEntity
            {
                Id = RockNightEventId,
                Name = "Rock Night Sarajevo",
                Description = "Open air concert",
                ScheduledDate = Clock.GetUtcNow().UtcDateTime.AddDays(30),
                OrganizerId = OrganizerId,
                VenueId = VenueId,
                EventTypeId = EventTypeId
            },
            new EventEntity
            {
                Id = SummerFestivalEventId,
                Name = "Mostar Summer Festival",
                Description = "Three days of live music",
                ScheduledDate = Clock.GetUtcNow().UtcDateTime.AddDays(60),
                OrganizerId = OtherOrganizerId,
                VenueId = OtherVenueId,
                EventTypeId = OtherEventTypeId
            });

        Db.EventNews.AddRange(
            new EventNewsEntity
            {
                Id = RockNightNewsId,
                OrganizerId = OrganizerId,
                EventId = RockNightEventId,
                Header = "Doors open at 19:00",
                Body = "Gates open one hour before the first act.",
                Image = SeededNewsImage
            },
            new EventNewsEntity
            {
                Id = RockNightSecondNewsId,
                OrganizerId = OrganizerId,
                EventId = RockNightEventId,
                Header = "Support act announced",
                Body = "A local band joins the line-up.",
                Image = null
            },
            new EventNewsEntity
            {
                Id = SummerFestivalNewsId,
                OrganizerId = OtherOrganizerId,
                EventId = SummerFestivalEventId,
                Header = "Camping site is open",
                Body = "The camping site opens the day before the festival.",
                Image = null
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

    public async Task<EventNewsEntity> AddEventNewsAsync(
        string header = "Seeded Event News",
        string? body = "Seeded event news body",
        int organizerId = OrganizerId,
        int eventId = RockNightEventId,
        string? image = null)
    {
        await using var seedContext = NewContext();

        var entity = new EventNewsEntity
        {
            OrganizerId = organizerId,
            EventId = eventId,
            Header = header,
            Body = body,
            Image = image
        };

        seedContext.EventNews.Add(entity);
        await seedContext.SaveChangesAsync(CancellationToken.None);

        return entity;
    }

    public async Task<EventNewsEntity?> GetEventNewsAsync(int id)
    {
        await using var readContext = NewContext();
        return await readContext.EventNews
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<EventNewsEntity>> GetEventNewsForEventAsync(int eventId)
    {
        await using var readContext = NewContext();
        return await readContext.EventNews
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
