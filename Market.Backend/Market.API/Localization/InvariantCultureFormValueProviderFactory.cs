using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace Market.API.Localization;

public sealed class InvariantCultureFormValueProviderFactory : IValueProviderFactory
{
    public async Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var request = context.ActionContext.HttpContext.Request;
        if (!request.HasFormContentType)
            return;

        var form = await request.ReadFormAsync(context.ActionContext.HttpContext.RequestAborted);

        context.ValueProviders.Insert(0, new FormValueProvider(
            BindingSource.Form,
            form,
            CultureInfo.InvariantCulture));
    }
}
