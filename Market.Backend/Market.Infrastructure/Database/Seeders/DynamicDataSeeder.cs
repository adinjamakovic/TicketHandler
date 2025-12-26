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
        await SeedEventTypes(context);
        await SeedGenresAsync(context);
        await SeedPerformersAsync(context);
        await SeedLocationsAsync(context);
        await SeedVenuesAsync(context);
        await SeedOrganizersAsync(context);
        await SeedLoyalityProgrammesAsync(context);
        await SeedEventsAsync(context);

    }

    private static async Task SeedCities(DatabaseContext context)
    {
        if (await context.Cities.AnyAsync())
            return;

        var Sarajevo = new CityEntity
        {
            Name = "Sarajevo",
            CountryId = 1,
            PostalCode = "71000",
            CreatedAtUtc = DateTime.UtcNow
        };
        var Muscat = new CityEntity
        {
            Name = "Muscat",
            CountryId = 5,
            PostalCode = "113",
            CreatedAtUtc = DateTime.UtcNow
        };
        var Ljubljana = new CityEntity
        {
            Name = "Ljubljana",
            CountryId = 4,
            PostalCode = "1000",
            CreatedAtUtc = DateTime.UtcNow
        };
        var Malmo = new CityEntity
        {
            Name = "Malmo",
            CountryId = 3,
            PostalCode = "200 02",
            CreatedAtUtc = DateTime.UtcNow
        };
        var Washington = new CityEntity
        {
            Name = "Washington D.C.",
            CountryId = 2,
            PostalCode = "20001",
            CreatedAtUtc = DateTime.UtcNow
        };
        var Zagreb = new CityEntity
        {
            Name = "Zagreb",
            CountryId = 6,
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
            CityId = 6,
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
            CityId = 6,
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
    /// <summary>
    /// Kreira demo organizatore ako ih još nema u bazi.
    /// </summary>
    private static async Task SeedOrganizersAsync(DatabaseContext context)
    {
        if (await context.Organizers.AnyAsync())
            return;

        var mkg = new OrganizerEntity
        {
            Name = "MKG",
            Description = "Dummy desc",
            Address = "Dummy address",
            CityId = 2,
            UserId = 2,
            CreatedAtUtc = DateTime.UtcNow,
            Logo = new byte[0]
        };
        
      
        context.Organizers.AddRange(mkg);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Dynamic seed: demo organizers added.");
    }
    /// <summary>
    /// Kreira demo korisnike ako ih još nema u bazi.
    /// </summary>
    private static async Task SeedGenresAsync(DatabaseContext context)
    {
        if (!await context.Genres.AnyAsync())
        {
            context.Genres.AddRange(
                new GenreEntity
                {
                    Name = "Rock",
                    Description = "Dummy desc",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new GenreEntity
                {
                    Name = "Pop",
                    Description = "Dummy desc",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new GenreEntity
                {
                    Name = "Metal",
                    Description = "Dummy desc",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new GenreEntity
                {
                    Name = "Folk",
                    Description = "Dummy desc",
                    CreatedAtUtc = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: Genres added.");
        }
    }

    private static async Task SeedPerformersAsync(DatabaseContext context)
    {
        if (!await context.Performers.AnyAsync())
        {
            context.Performers.AddRange(
                new PerformerEntity
                {
                    Name = "Queen",
                    Image = new byte[0],
                    Description = "Dummy desc",
                    GenreId = 1,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new PerformerEntity
                {
                    Name = "Taylor Swift",
                    Image = new byte[0],
                    Description = "Dummy desc",
                    GenreId = 2,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new PerformerEntity
                {
                    Name = "Rammstein",
                    Image = new byte[0],
                    Description = "Dummy desc",
                    GenreId = 3,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new PerformerEntity
                {
                    Name = "Bob Dylan",
                    Image = new byte[0],
                    Description = "Dummy desc",
                    GenreId = 4,
                    CreatedAtUtc = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: Performers added.");
        }
    }
    private static async Task SeedVenuesAsync(DatabaseContext context)
    {
        if (!await context.Venues.AnyAsync())
        {
            context.Venues.AddRange(
                new VenueEntity
                {
                    Name = "The Nave",
                    Seated =5000,
                    Standing=150,
                    LocationId = 1,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new VenueEntity
                {
                    Name = "Musee National d Art Moderne",
                    Seated = 0,
                    Standing = 500,
                    LocationId = 2,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new VenueEntity
                {
                    Name = "Alianz Arena Event Area",
                    Seated = 500,
                    Standing = 400,
                    LocationId = 3,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new VenueEntity
                {
                    Name = "Maddison Square Garden - Theater",
                    Seated = 5000,
                    Standing = 2000,
                    LocationId = 4,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new VenueEntity
                {
                    Name = "Sydney Opera House - Concert Hall",
                    Seated = 10000,
                    Standing = 0,
                    LocationId = 5,
                    CreatedAtUtc = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: Venues added.");
        }
    }
    private static async Task SeedLocationsAsync(DatabaseContext context)
    {
        if (!await context.Locations.AnyAsync())
        {
            context.Locations.AddRange(
                new LocationEntity
                {
                   Name = "Grand Palais",
                   CityId = 1,
                   Address="Dummy Address",
                   Description="Dummy Desc",
                   CreatedAtUtc= DateTime.UtcNow
                },
                new LocationEntity
                {
                    Name = "Pompidou Centre",
                    CityId = 2,
                    Address = "Dummy Address",
                    Description = "Dummy Desc",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new LocationEntity
                {
                    Name = "Alianz Arena",
                    CityId = 3,
                    Address = "Dummy Address",
                    Description = "Dummy Desc",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new LocationEntity
                {
                    Name = "Maddison Square Garden",
                    CityId = 4,
                    Address = "Dummy Address",
                    Description = "Dummy Desc",
                    CreatedAtUtc = DateTime.UtcNow
                },
                new LocationEntity
                {
                    Name = "Sydney Opera House",
                    CityId = 5,
                    Address = "Dummy Address",
                    Description = "Dummy Desc",
                    CreatedAtUtc = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: Venues added.");
        }
    }
    private static async Task SeedLoyalityProgrammesAsync(DatabaseContext context)
    {
        if (!await context.LoyaltyProgrammes.AnyAsync())
        {
            context.LoyaltyProgrammes.AddRange(
                new LoyaltyProgrammeEntity
                {
                    Name = "Regular Customer",
                    MinRange = 0,
                    MaxRange = 100,
                    Discount = 0.05m
                },
                new LoyaltyProgrammeEntity
                {
                    Name = "Loyal Customer",
                    MinRange = 101,
                    MaxRange = 10000,
                    Discount = 0.2m
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: Venues added.");
        }
    }
    private static async Task SeedEventsAsync(DatabaseContext context)
    {
        if (!await context.Events.AnyAsync())
        {
            context.Events.AddRange(
                new EventEntity
                {
                    Name="Dummy Concert",
                    Description="Dummy Desc",
                    ScheduledDate= DateTime.Now,
                    OrganizerId = 1,
                    VenueId=1,
                    Image=new byte[0],
                    EventTypeId=1,
                },
                new EventEntity
                {
                    Name = "Dummy Festival",
                    Description = "Dummy Desc",
                    ScheduledDate = DateTime.Now.AddDays(10),
                    OrganizerId = 1,
                    VenueId = 2,
                    Image = new byte[0],
                    EventTypeId = 2,
                },
                new EventEntity
                {
                    Name = "Dummy Movie",
                    Description = "Dummy Desc",
                    ScheduledDate = DateTime.Now.AddDays(20),
                    OrganizerId = 1,
                    VenueId = 2,
                    Image = new byte[0],
                    EventTypeId = 3,
                },
                new EventEntity
                {
                    Name = "Dummy Screen Play",
                    Description = "Dummy Desc",
                    ScheduledDate = DateTime.Now.AddDays(15),
                    OrganizerId = 1,
                    VenueId = 3,
                    Image = new byte[0],
                    EventTypeId = 4,
                },
                new EventEntity
                {
                    Name = "Dummy Theater Act",
                    Description = "Dummy Desc",
                    ScheduledDate = DateTime.Now.AddDays(30),
                    OrganizerId = 1,
                    VenueId = 4,
                    Image = new byte[0],
                    EventTypeId = 5,
                },
                new EventEntity
                {
                    Name = "Dummy Opera/Ballet",
                    Description = "Dummy Desc",
                    ScheduledDate = DateTime.Now.AddDays(1),
                    OrganizerId = 1,
                    VenueId = 4,
                    Image = new byte[0],
                    EventTypeId = 6,
                }
            );

            await context.SaveChangesAsync();
            Console.WriteLine("✅ Dynamic seed: Venues added.");
        }
    }
}