using Market.API.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace Market.Tests.LocalizationTests.UnitTests;

public class InvariantCultureFormValueProviderFactoryTests
{
    private static async Task<IValueProvider> FormValueProviderFor(
        Dictionary<string, StringValues> form,
        CultureInfo requestCulture)
    {
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = requestCulture;
        try
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
            httpContext.Request.Form = new FormCollection(form);

            var actionContext = new Microsoft.AspNetCore.Mvc.ActionContext(
                httpContext, new RouteData(), new ActionDescriptor());
            var context = new ValueProviderFactoryContext(actionContext);

            await new InvariantCultureFormValueProviderFactory().CreateValueProviderAsync(context);

            return Assert.Single(context.ValueProviders);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public async Task CreateValueProviderAsync_ReadsFormValuesWithTheInvariantCulture()
    {
        var provider = await FormValueProviderFor(
            new Dictionary<string, StringValues> { ["UnitPrice"] = "19.99" },
            CultureInfo.GetCultureInfo("bs-BA"));

        var result = provider.GetValue("UnitPrice");

        Assert.Equal(CultureInfo.InvariantCulture, result.Culture);
        Assert.Equal(19.99m, decimal.Parse(result.FirstValue!, result.Culture));
    }

    [Fact]
    public async Task CreateValueProviderAsync_BindsIsoDatesRegardlessOfRequestCulture()
    {
        var provider = await FormValueProviderFor(
            new Dictionary<string, StringValues> { ["ScheduledDate"] = "2026-09-18T20:00:00Z" },
            CultureInfo.GetCultureInfo("bs-BA"));

        var result = provider.GetValue("ScheduledDate");

        var parsed = DateTime.Parse(
            result.FirstValue!, result.Culture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);

        Assert.Equal(new DateTime(2026, 9, 18, 20, 0, 0, DateTimeKind.Utc), parsed);
    }

    [Fact]
    public async Task CreateValueProviderAsync_WithoutFormContentType_AddsNoProvider()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.ContentType = "application/json";

        var actionContext = new Microsoft.AspNetCore.Mvc.ActionContext(
            httpContext, new RouteData(), new ActionDescriptor());
        var context = new ValueProviderFactoryContext(actionContext);

        await new InvariantCultureFormValueProviderFactory().CreateValueProviderAsync(context);

        Assert.Empty(context.ValueProviders);
    }
}
