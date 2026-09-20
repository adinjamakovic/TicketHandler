using Market.Shared.Options;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Market.API.Localization;

public static class RegionalizationSetup
{
    public static IServiceCollection AddRegionalization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var regional = configuration.GetSection(RegionalOptions.SectionName).Get<RegionalOptions>()
                       ?? new RegionalOptions();

        services.Configure<RegionalOptions>(configuration.GetSection(RegionalOptions.SectionName));

        var localization = BuildLocalizationOptions(regional);

        var defaultCulture = localization.DefaultRequestCulture.Culture;
        CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
        CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

        services.Configure<RequestLocalizationOptions>(o =>
        {
            o.DefaultRequestCulture = localization.DefaultRequestCulture;
            o.SupportedCultures = localization.SupportedCultures;
            o.SupportedUICultures = localization.SupportedUICultures;
            o.RequestCultureProviders = localization.RequestCultureProviders;
            o.FallBackToParentCultures = true;
            o.FallBackToParentUICultures = true;
            o.ApplyCurrentCultureToResponseHeaders = true;
        });

        return services;
    }

    public static RequestLocalizationOptions BuildLocalizationOptions(RegionalOptions regional)
    {
        var supported = regional.SupportedCultures
            .Select(Resolve)
            .ToList();

        var defaultCulture = Resolve(regional.DefaultCulture);
        if (!supported.Any(c => c.Name == defaultCulture.Name))
            supported.Insert(0, defaultCulture);

        return new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(defaultCulture, defaultCulture),
            SupportedCultures = supported,
            SupportedUICultures = supported,
            FallBackToParentCultures = true,
            FallBackToParentUICultures = true,
            ApplyCurrentCultureToResponseHeaders = true,
            RequestCultureProviders =
            [
                new QueryStringRequestCultureProvider { QueryStringKey = regional.QueryStringKey },
                new AcceptLanguageHeaderRequestCultureProvider()
            ]
        };
    }

    private static CultureInfo Resolve(string name)
    {
        try
        {
            return CultureInfo.GetCultureInfo(name, predefinedOnly: true);
        }
        catch (CultureNotFoundException ex)
        {
            throw new InvalidOperationException(
                $"Configuration '{RegionalOptions.SectionName}' references unknown culture '{name}'.", ex);
        }
    }
}
