using Market.Application.Abstractions;
using Market.Application.Common.Behaviors;
using Market.Application.Common.Services;
using Market.Application.Modules.Sales.Payments;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Market.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR only from the Application layer
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation from the Application layer
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline behaviors (e.g. ValidationBehavior)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // TimeProvider — if used by handlers
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IImageCompressor, ImageCompressor>();
        services.AddScoped<IImageStorage, ImageStorage>();

        // Shared by the checkout confirmation and the Stripe webhook, which both settle payments.
        services.AddScoped<PaymentSettlementService>();

        return services;
    }
}