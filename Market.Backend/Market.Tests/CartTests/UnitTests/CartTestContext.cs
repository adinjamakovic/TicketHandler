using Market.Domain.Entities.Events;
using Market.Domain.Entities.Geographical;
using Market.Domain.Entities.Identity;
using Market.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Time.Testing;

namespace Market.Tests.CartTests.UnitTests;

public sealed class CartTestContext : IAsyncDisposable
{
    public const int CountryId = 1;
    public const int SarajevoCityId = 1;
    public const int OrganizerUserId = 1;
    public const int OrganizerId = 1;
    public const int VenueId = 1;
    public const int EventTypeId = 1;
    public const int RockNightEventId = 1;
    public const int RegularTicketTypeId = 1;
    public const int VipTicketTypeId = 2;

    /// <summary>The person whose cart the tests act on.</summary>
    public const int BuyerId = 10;

    /// <summary>A second buyer, used to prove carts don't leak across people.</summary>
    public const int OtherBuyerId = 11;

    public const int MissingId = 9999;

    private static readonly InMemoryDatabaseRoot Root = new();
    private readonly DbContextOptions<DatabaseContext> _options;
    private readonly List<DatabaseContext> _contexts = [];
    public DatabaseContext Db { get; }
    public FakeTimeProvider Clock { get; }

    private CartTestContext()
    {
        Clock = new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.Zero));

        _options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase($"cart-{Guid.NewGuid()}", Root)
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

    public static async Task<CartTestContext> CreateAsync()
    {
        var ctx = new CartTestContext();
        await ctx.SeedAsync();
        return ctx;
    }

    private async Task SeedAsync()
    {
        Db.Countries.Add(new CountryEntity { Id = CountryId, Name = "Bosnia and Herzegovina", IsoCode = "BA", PhoneCode = "+387" });

        Db.Cities.Add(new CityEntity { Id = SarajevoCityId, CountryId = CountryId, Name = "Sarajevo", PostalCode = "71000" });

        Db.Locations.Add(new LocationEntity { Id = 1, CityId = SarajevoCityId, Name = "Skenderija", Address = "Terezije bb", Description = "Sports and culture centre" });

        Db.Venues.Add(new VenueEntity { Id = VenueId, LocationId = 1, Name = "Mirza Delibasic Hall", Seated = 5000, Standing = 2000 });

        Db.EventTypes.Add(new EventTypeEntity { Id = EventTypeId, Name = "Concert", IsEnabled = true });

        Db.Persons.AddRange(
            NewOrganiser(OrganizerUserId, "organiser.one"),
            NewBuyer(BuyerId, "buyer.one"),
            NewBuyer(OtherBuyerId, "buyer.two"));

        Db.Organizers.Add(new OrganizerEntity
        {
            Id = OrganizerId,
            UserId = OrganizerUserId,
            CityId = SarajevoCityId,
            Name = "Sarajevo Events",
            Description = "Primary organizer",
            Address = "Marsala Tita 1"
        });

        Db.Events.Add(new EventEntity
        {
            Id = RockNightEventId,
            Name = "Rock Night Sarajevo",
            Description = "Open air concert",
            ScheduledDate = Clock.GetUtcNow().UtcDateTime.AddDays(30),
            OrganizerId = OrganizerId,
            VenueId = VenueId,
            EventTypeId = EventTypeId
        });

        Db.TicketTypes.AddRange(
            new TicketTypeEntity { Id = RegularTicketTypeId, Name = "Regular", Description = "Standing area" },
            new TicketTypeEntity { Id = VipTicketTypeId, Name = "VIP", Description = "Front row" });

        await Db.SaveChangesAsync(CancellationToken.None);
        Db.ChangeTracker.Clear();
    }

    private static PersonEntity NewOrganiser(int id, string userName) => new()
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

    private static PersonEntity NewBuyer(int id, string userName) => new()
    {
        Id = id,
        CityId = SarajevoCityId,
        FirstName = "Test",
        LastName = "Buyer",
        BirthDate = new DateTime(1995, 6, 1),
        UserName = userName,
        Email = $"{userName}@test.local",
        PasswordHash = "hash",
        IsUser = true,
        IsEnabled = true
    };

    public async Task<TicketsEntity> AddTicketAsync(
        int ticketTypeId = RegularTicketTypeId,
        decimal quantityInStock = 100,
        decimal unitPrice = 50,
        string benefits = "Entry")
    {
        await using var seedContext = NewContext();

        var entity = new TicketsEntity
        {
            EventId = RockNightEventId,
            TicketTypeId = ticketTypeId,
            QuantityInStock = quantityInStock,
            UnitPrice = unitPrice,
            Benefits = benefits
        };

        seedContext.Tickets.Add(entity);
        await seedContext.SaveChangesAsync(CancellationToken.None);

        return entity;
    }

    public async Task<CartItemEntity> AddCartItemAsync(
        int ticketId,
        int personId = BuyerId,
        decimal quantity = 2,
        bool isSavedForLater = false)
    {
        await using var seedContext = NewContext();

        var entity = new CartItemEntity
        {
            PersonId = personId,
            TicketId = ticketId,
            Quantity = quantity,
            IsSavedForLater = isSavedForLater,
            CreatedAtUtc = Clock.GetUtcNow().UtcDateTime,
            ModifiedAtUtc = Clock.GetUtcNow().UtcDateTime
        };

        seedContext.CartItems.Add(entity);
        await seedContext.SaveChangesAsync(CancellationToken.None);

        return entity;
    }

    /// <summary>Reads a line back ignoring the soft-delete filter, so removals stay assertable.</summary>
    public async Task<CartItemEntity?> GetCartItemAsync(int ticketId, int personId = BuyerId)
    {
        await using var readContext = NewContext();
        return await readContext.CartItems
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.PersonId == personId && x.TicketId == ticketId);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var context in _contexts)
            await context.DisposeAsync();
    }
}
