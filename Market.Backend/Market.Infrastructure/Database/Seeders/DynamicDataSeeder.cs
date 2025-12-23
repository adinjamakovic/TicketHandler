namespace Market.Infrastructure.Database.Seeders;

/// <summary>
/// Dynamic seeder koji se pokreće u runtime-u,
/// obično pri startu aplikacije (npr. u Program.cs).
/// Koristi se za unos demo/test podataka koji nisu dio migracije.
/// </summary>
public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        // Osiguraj da baza postoji (bez migracija)
        await context.Database.EnsureCreatedAsync();

        await SeedCountries(context);
        await SeedCities(context);
        await SeedUsersAsync(context);
        //await SeedEventTypes(context);
    }

    private static async Task SeedCities(DatabaseContext context)
    {
            if (await context.Cities.AnyAsync())
                return;

            var Sarajevo = new CityEntity
            {
                Name = "Sarajevo",
                CountryId = 2,
                PostalCode = "71000",
                CreatedAtUtc = DateTime.UtcNow
            };
            var Muscat = new CityEntity
            {
                Name = "Muscat",
                CountryId = 6,
                PostalCode = "113",
                CreatedAtUtc = DateTime.UtcNow
            };
            var Ljubljana = new CityEntity
            {
                Name = "Ljubljana",
                CountryId = 5,
                PostalCode = "1000",
                CreatedAtUtc = DateTime.UtcNow
            };
            var Malmo = new CityEntity
            {
                Name = "Malmo",
                CountryId = 4,
                PostalCode = "200 02",
                CreatedAtUtc = DateTime.UtcNow
            };
            var Washington = new CityEntity
            {
                Name = "Washington D.C.",
                CountryId = 3,
                PostalCode = "20001",
                CreatedAtUtc = DateTime.UtcNow
            };
            var Zagreb = new CityEntity
            {
                Name = "Zagreb",
                CountryId = 7,
                PostalCode = "10020",
                CreatedAtUtc = DateTime.UtcNow
            };
            context.Cities.AddRange(Zagreb, Washington, Malmo, Ljubljana, Muscat, Sarajevo);
        
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: Cities added.");
    }
    
    

    private static async Task SeedCountries(DatabaseContext context)
    {
        if (!await context.Countries.AnyAsync())
        {
            context.Countries.AddRange(
                new CountryEntity
                {
                    Name = "Bosnia and Herzegovina",
                    PhoneCode = "+387",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new CountryEntity
                {
                    Name = "United States of America",
                    PhoneCode = "+1",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new CountryEntity
                {
                    Name = "Sweden",
                    PhoneCode = "+46",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new CountryEntity
                {
                    Name = "Slovenia",
                    PhoneCode = "+386",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new CountryEntity
                {
                    Name = "Oman",
                    PhoneCode = "+968",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new CountryEntity
                {
                    Name = "Croatia",
                    PhoneCode = "+385",
                    CreatedAtUtc = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: Countries added.");
        }
    }

    private static async Task SeedEventTypes(DatabaseContext context)
    {
        if (!await context.EventTypes.AnyAsync())
        {
            context.EventTypes.AddRange(
                new EventTypeEntity
                {
                    Name = "Concert",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new EventTypeEntity
                {
                    Name = "Festival",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new EventTypeEntity
                {
                    Name = "Movie",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new EventTypeEntity
                {
                    Name = "Screen play",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new EventTypeEntity
                {
                    Name = "Theater Act",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new EventTypeEntity
                {
                    Name = "Opera/Ballet",
                    IsEnabled = true,
                    CreatedAtUtc = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: product categories added.");
        }
    }

    /// <summary>
    /// Kreira demo korisnike ako ih još nema u bazi.
    /// </summary>
    private static async Task SeedUsersAsync(DatabaseContext context)
    {
        if (await context.Persons.AnyAsync())
            return;

        var hasher = new PasswordHasher<PersonEntity>();

        var admin = new PersonEntity
        {
            CityId = 6,
            Address = "Dummy Address",
            Email = "admin@market.local",
            PasswordHash = hasher.HashPassword(null!, "Admin123!"),
            IsAdmin = true,
            IsEnabled = true,
        };

        var user = new PersonEntity
        {
            CityId=6,
            Address = "Dummy Address",
            Email = "dummy_organiser@market.local",
            PasswordHash = hasher.HashPassword(null!, "User123!"),
            IsOrganiser = true,
            IsEnabled = true,
        };

        var dummyForSwagger = new PersonEntity
        {
            CityId = 6,
            Address = "Dummy Address",
            Email = "string",
            PasswordHash = hasher.HashPassword(null!, "string"),
            IsAdmin = true,
            IsEnabled = true,
        };
        var dummyForTests = new PersonEntity
        {

            Address = "Dummy Address",
            Email = "test",
            PasswordHash = hasher.HashPassword(null!, "test123"),
            IsUser = true,
            IsEnabled = true,
        };
        context.Persons.AddRange(admin, user, dummyForSwagger, dummyForTests);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Dynamic seed: demo users added.");
    }
}