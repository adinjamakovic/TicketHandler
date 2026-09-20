using Market.Domain.Entities.Geographical;
using Market.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Time.Testing;

namespace Market.Tests.PersonTests.UnitTests;

public sealed class PersonTestContext : IAsyncDisposable
{
    public const int CountryId = 1;
    public const int SarajevoCityId = 1;
    public const int OwnerPersonId = 1;
    public const int OtherPersonId = 2;
    public const int MissingId = 9999;

    private static readonly InMemoryDatabaseRoot Root = new();
    private readonly DbContextOptions<DatabaseContext> _options;
    private readonly List<DatabaseContext> _contexts = [];
    public DatabaseContext Db { get; }
    public FakeTimeProvider Clock { get; }

    private PersonTestContext()
    {
        Clock = new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.Zero));

        _options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase($"person-{Guid.NewGuid()}", Root)
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

    public static async Task<PersonTestContext> CreateAsync()
    {
        var ctx = new PersonTestContext();
        await ctx.SeedAsync();
        return ctx;
    }

    private async Task SeedAsync()
    {
        Db.Countries.Add(new CountryEntity { Id = CountryId, Name = "Bosnia and Herzegovina", IsoCode = "BA", PhoneCode = "+387" });
        Db.Cities.Add(new CityEntity { Id = SarajevoCityId, CountryId = CountryId, Name = "Sarajevo", PostalCode = "71000" });

        Db.Persons.AddRange(
            NewPerson(OwnerPersonId, "owner", "+38761111111", "Marsala Tita 1"),
            NewPerson(OtherPersonId, "victim", "+38762222222", "Ferhadija 5"));

        await Db.SaveChangesAsync(CancellationToken.None);
        Db.ChangeTracker.Clear();
    }

    private static PersonEntity NewPerson(int id, string userName, string phone, string address) => new()
    {
        Id = id,
        CityId = SarajevoCityId,
        FirstName = "Test",
        LastName = userName,
        BirthDate = new DateTime(1990, 1, 1),
        UserName = userName,
        Email = $"{userName}@test.local",
        Address = address,
        Gender = "M",
        Phone = phone,
        PasswordHash = "hash",
        IsUser = true,
        IsEnabled = true
    };

    public async ValueTask DisposeAsync()
    {
        foreach (var context in _contexts)
            await context.DisposeAsync();
    }
}
