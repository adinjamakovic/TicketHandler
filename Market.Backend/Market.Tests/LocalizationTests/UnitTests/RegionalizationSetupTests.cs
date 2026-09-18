using Market.API.Localization;
using Market.Shared.Options;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Market.Tests.LocalizationTests.UnitTests;

public class RegionalizationSetupTests
{
    [Fact]
    public void BuildLocalizationOptions_WithDefaults_UsesBosnianAsDefaultCulture()
    {
        var options = RegionalizationSetup.BuildLocalizationOptions(new RegionalOptions());

        Assert.Equal("bs-BA", options.DefaultRequestCulture.Culture.Name);
        Assert.Equal("bs-BA", options.DefaultRequestCulture.UICulture.Name);
    }

    [Fact]
    public void BuildLocalizationOptions_KeepsConfiguredSupportedCultures()
    {
        var regional = new RegionalOptions { SupportedCultures = ["bs-BA", "hr-HR", "en-US"] };

        var options = RegionalizationSetup.BuildLocalizationOptions(regional);

        Assert.Equal(
            ["bs-BA", "hr-HR", "en-US"],
            options.SupportedCultures!.Select(c => c.Name));
        Assert.Equal(
            options.SupportedCultures!.Select(c => c.Name),
            options.SupportedUICultures!.Select(c => c.Name));
    }

    [Fact]
    public void BuildLocalizationOptions_WhenDefaultIsNotListed_AddsIt()
    {
        var regional = new RegionalOptions
        {
            DefaultCulture = "bs-BA",
            SupportedCultures = ["en-US"]
        };

        var options = RegionalizationSetup.BuildLocalizationOptions(regional);

        Assert.Contains(options.SupportedCultures!, c => c.Name == "bs-BA");
    }

    [Fact]
    public void BuildLocalizationOptions_NegotiatesCultureFromQueryStringThenAcceptLanguage()
    {
        var regional = new RegionalOptions { QueryStringKey = "culture" };

        var options = RegionalizationSetup.BuildLocalizationOptions(regional);

        var providers = options.RequestCultureProviders;
        var queryString = Assert.IsType<QueryStringRequestCultureProvider>(providers[0]);
        Assert.Equal("culture", queryString.QueryStringKey);
        Assert.IsType<AcceptLanguageHeaderRequestCultureProvider>(providers[1]);
        Assert.True(options.ApplyCurrentCultureToResponseHeaders);
    }

    [Fact]
    public void BuildLocalizationOptions_WithUnknownCulture_ThrowsWithSectionName()
    {
        var regional = new RegionalOptions { DefaultCulture = "zz-ZZ" };

        var ex = Assert.Throws<InvalidOperationException>(
            () => RegionalizationSetup.BuildLocalizationOptions(regional));

        Assert.Contains(RegionalOptions.SectionName, ex.Message);
        Assert.Contains("zz-ZZ", ex.Message);
    }

    [Fact]
    public void DefaultCulture_FormatsMoneyAndDatesTheWayTheClientDoes()
    {
        var options = RegionalizationSetup.BuildLocalizationOptions(new RegionalOptions());
        var culture = options.DefaultRequestCulture.Culture;

        // The Angular client renders prices as "KM" and uses a comma decimal separator,
        // so anything the backend formats (logs, e-mails, validation messages) matches.
        Assert.Equal("KM", culture.NumberFormat.CurrencySymbol);
        Assert.Equal(",", culture.NumberFormat.NumberDecimalSeparator);
        Assert.Equal("19,99", 19.99m.ToString("N2", culture));
        // Day-first, dot separated - never the US m/d/yyyy the server locale might impose.
        Assert.Equal("18. 9. 2026.", new DateTime(2026, 9, 18).ToString("d", culture));
    }

    [Fact]
    public void InvariantParsing_IsUnaffectedByTheRequestCulture()
    {
        var culture = RegionalizationSetup
            .BuildLocalizationOptions(new RegionalOptions())
            .DefaultRequestCulture.Culture;

        // The wire format stays invariant no matter which culture a request negotiates:
        // parsing with bs-BA would read "19.99" as 1999.
        Assert.Equal(19.99m, decimal.Parse("19.99", CultureInfo.InvariantCulture));
        Assert.Equal(1999m, decimal.Parse("19.99", culture));
    }
}
