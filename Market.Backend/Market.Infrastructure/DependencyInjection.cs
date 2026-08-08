using Market.Application.Abstractions;
using Market.Application.Abstractions.Payments;
using Market.Infrastructure.Common;
using Market.Infrastructure.Database;
using Market.Shared.Constants;
using Market.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Market.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        // Typed ConnectionStrings + validation
        services.AddOptions<ConnectionStringsOptions>()
            .Bind(configuration.GetSection(ConnectionStringsOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // DbContext: InMemory for test environments; SQL Server otherwise
        services.AddDbContext<DatabaseContext>((sp, options) =>
        {
            if (env.IsTest())
            {
                options.UseInMemoryDatabase("IntegrationTestsDb");

                return;
            }

            var cs = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value.Main;
            options.UseSqlServer(cs);
        });

        // IAppDbContext mapping
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<DatabaseContext>());

        // Identity hasher
        services.AddScoped<IPasswordHasher<PersonEntity>, PasswordHasher<PersonEntity>>();

        services.AddOptions<OpenAiOptions>()
            .Bind(configuration.GetSection(OpenAiOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Consumed by the Application layer's ImageCompressor; bound here because this is where
        // configuration is available.
        services.AddOptions<ImageCompressionOptions>()
            .Bind(configuration.GetSection(ImageCompressionOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Stripe. Bound without ValidateOnStart so the API still boots with empty keys —
        // only the checkout endpoints fail, and they say why.
        services.AddOptions<StripeOptions>()
            .Bind(configuration.GetSection(StripeOptions.SectionName));

        services.AddScoped<IPaymentGateway, StripePaymentGateway>();

        services.AddHttpClient<IAiCompletionService, OpenAiCompletionService>((sp, client) =>
        {
            var openAi = sp.GetRequiredService<IOptions<OpenAiOptions>>().Value;

            client.BaseAddress = new Uri(openAi.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(openAi.TimeoutSeconds);

            if (!string.IsNullOrWhiteSpace(openAi.ApiKey))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", openAi.ApiKey);
        });

        // HttpContext accessor + current user
        services.AddHttpContextAccessor();
        services.AddScoped<IAppCurrentUser, AppCurrentUser>();

        // TimeProvider (if used in handlers/services)
        services.AddSingleton<TimeProvider>(TimeProvider.System);

        return services;
    }
}