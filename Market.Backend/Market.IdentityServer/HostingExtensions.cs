using System.Globalization;
using FluentValidation;
using Market.Application.Abstractions;
using Market.Application.Common;
using Market.Application.Common.Behaviors;
using Market.Application.Modules.Geography.Cities.Queries.List;
using Market.Application.Modules.Identity.Person.Commands.Create;
using Market.Domain.Entities.Identity;
using Market.IdentityServer.Services;
using Market.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;

namespace Market.IdentityServer;

internal static class HostingExtensions
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        // Set up logging to write regular entries to console, and diagnostics data to a file.
        // See https://duende.link/diagnostics
        _ = builder.Services.AddSerilog(lc =>
        {
            _ = lc.WriteTo.Logger(consoleLogger =>
            {
                _ = consoleLogger.WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    formatProvider: CultureInfo.InvariantCulture);
                if (builder.Environment.IsDevelopment())
                {
                    _ = consoleLogger.Filter.ByExcluding(Matching.FromSource("Duende.IdentityServer.Diagnostics.Summary"));
                }
            });
            if (builder.Environment.IsDevelopment())
            {
                _ = lc.WriteTo.Logger(fileLogger =>
                {
                    _ = fileLogger
                        .WriteTo.File("./diagnostics/diagnostic.log", rollingInterval: RollingInterval.Day,
                            fileSizeLimitBytes: 1024 * 1024 * 10, // 10 MB
                            rollOnFileSizeLimit: true,
                            outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                            formatProvider: CultureInfo.InvariantCulture)
                        .Filter
                        .ByIncludingOnly(Matching.FromSource("Duende.IdentityServer.Diagnostics.Summary"));
                }).Enrich.FromLogContext().ReadFrom.Configuration(builder.Configuration);
            }
        });
        return builder;
    }

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        var connectionString = builder.Configuration.GetConnectionString("Main");
        builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
        builder.Services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddScoped<IPasswordHasher<PersonEntity>, PasswordHasher<PersonEntity>>();
        builder.Services.AddScoped<PersonCredentialStore>();

        builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<DatabaseContext>());

        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(HostingExtensions).Assembly));

        builder.Services
            .AddTransient<IRequestHandler<CreatePersonCommand, int>, CreatePersonCommandHandler>();
        builder.Services
            .AddTransient<IRequestHandler<ListCitiesQuery, PageResult<ListCitiesQueryDto>>, ListCitiesQueryHandler>();

        builder.Services.AddScoped<IValidator<CreatePersonCommand>, CreatePersonCommandValidator>();
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        _ = builder.Services.AddIdentityServer(options =>
            {
                options.UserInteraction.CreateAccountUrl = "/Account/Create";
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryClients(Config.Clients)
            .AddProfileService<PersonProfileService>()
            .AddLicenseSummary();

        // add `.PersistKeysTo…()` and `.ProtectKeysWith…()` calls
        // see more at https://docs.duendesoftware.com/general/data-protection
        _ = builder.Services.AddDataProtection()
                   .SetApplicationName("IdentityServer");

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        _ = app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            _ = app.UseDeveloperExceptionPage();
        }

        
        app.UseStaticFiles();
        app.UseRouting();

        _ = app.UseIdentityServer();

        app.UseAuthorization();
        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
