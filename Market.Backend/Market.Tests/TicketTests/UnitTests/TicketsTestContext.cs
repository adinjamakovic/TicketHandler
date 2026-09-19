using Market.Domain.Entities.Events;
using Market.Domain.Entities.Geographical;
using Market.Domain.Entities.Identity;
using Market.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Time.Testing;

namespace Market.Tests.TicketTests.UnitTests;

public sealed class TicketsTestContext : IAsyncDisposable
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

    /// <summary>Event run by <see cref="OrganizerId"/>.</summary>
    public const int RockNightEventId = 1;

    /// <summary>Event run by <see cref="OtherOrganizerId"/>.</summary>
    public const int SummerFestivalEventId = 2;
    public const int RegularTicketTypeId = 1;
    public const int VipTicketTypeId = 2;
    public const int MissingId = 9999;

    private static readonly InMemoryDatabaseRoot Root = new();
    private readonly DbContextOptions<DatabaseContext> _options;
    private readonly List<DatabaseContext> _contexts = [];
    public DatabaseContext Db { get; }
    public FakeTimeProvider Clock { get; }

    private TicketsTestContext()
    {
        Clock = new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.Zero));

        _options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase($"tickets-{Guid.NewGuid()}", Root)
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

    public static async Task<TicketsTestContext> CreateAsync()
    {
        var ctx = new TicketsTestContext();
        await ctx.SeedAsync();
        return ctx;
    }

    private async Task SeedAsync()
    {
        Db.Countries.Add(new CountryEntity { Id = CountryId, Name = "Bosnia and Herzegovina", IsoCode = "BA", PhoneCode = "+387" });

        Db.Cities.AddRange(
            new CityEntity { Id = SarajevoCityId, CountryId = CountryId, Name = "Sarajevo", PostalCode = "71000" },
            new CityEntity { Id = MostarCityId, CountryId = CountryId, Name = "Mostar", PostalCode = "88000" });

        Db.Locations.AddRange(
            new LocationEntity { Id = 1, CityId = SarajevoCityId, Name = "Skenderija", Address = "Terezije bb", Description = "Sports and culture centre" },
            new LocationEntity { Id = 2, CityId = MostarCityId, Name = "Rondo", Address = "Kralja Petra 1", Description = "City square" });

        Db.Venues.AddRange(
            new VenueEntity { Id = VenueId, LocationId = 1, Name = "Mirza Delibasic Hall", Seated = 5000, Standing = 2000 },
            new VenueEntity { Id = OtherVenueId, LocationId = 2, Name = "Mostar Open Air", Seated = 0, Standing = 8000 });

        Db.EventTypes.Add(new EventTypeEntity { Id = EventTypeId, Name = "Concert", IsEnabled = true });

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
                EventTypeId = EventTypeId
            });

        Db.TicketTypes.AddRange(
            new TicketTypeEntity { Id = RegularTicketTypeId, Name = "Regular", Description = "Standing area" },
            new TicketTypeEntity { Id = VipTicketTypeId, Name = "VIP", Description = "Front row" });

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

    public async Task<TicketsEntity> AddTicketAsync(
        int eventId = RockNightEventId,
        int ticketTypeId = RegularTicketTypeId,
        decimal quantityInStock = 100,
        decimal unitPrice = 50,
        string benefits = "Entry")
    {
        await using var seedContext = NewContext();

        var entity = new TicketsEntity
        {
            EventId = eventId,
            TicketTypeId = ticketTypeId,
            QuantityInStock = quantityInStock,
            UnitPrice = unitPrice,
            Benefits = benefits
        };

        seedContext.Tickets.Add(entity);
        await seedContext.SaveChangesAsync(CancellationToken.None);

        return entity;
    }

    public async Task<TicketsEntity?> GetTicketAsync(int id)
    {
        await using var readContext = NewContext();
        return await readContext.Tickets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> CountTicketsAsync()
    {
        await using var readContext = NewContext();
        return await readContext.Tickets.AsNoTracking().CountAsync();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var context in _contexts)
            await context.DisposeAsync();
    }
}
