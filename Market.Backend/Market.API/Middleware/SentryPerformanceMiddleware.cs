using Microsoft.AspNetCore.Mvc.Controllers;
using Sentry;
using System.Diagnostics;
using System.Security.Claims;

namespace Market.API.Middleware;
public sealed class SentryPerformanceMiddleware(RequestDelegate next)
{
    private const int SlowRequestThresholdMs = 400;

    public async Task InvokeAsync(HttpContext context)
    {
        var transaction = SentrySdk.GetSpan()?.GetTransaction();
        if (transaction is null)
        {
            await next(context);
            return;
        }

        var endpoint = context.GetEndpoint();
        var action = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

        transaction.SetTag("http.method", context.Request.Method);

        if (action is not null)
        {
            transaction.SetTag("controller", action.ControllerName);
            transaction.SetTag("action", action.ActionName);
        }

        if (endpoint is RouteEndpoint routeEndpoint)
            transaction.SetTag("http.route", routeEndpoint.RoutePattern.RawText ?? context.Request.Path.Value ?? "/");

        var userId = context.User.FindFirst("sub")?.Value
                     ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrWhiteSpace(userId))
            SentrySdk.ConfigureScope(scope => scope.User = new SentryUser { Id = userId });

        var stopwatch = Stopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();

            transaction.SetTag("http.status_code", context.Response.StatusCode.ToString());

            if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
            {
                transaction.SetTag("slow_request", "true");
                SentrySdk.AddBreadcrumb(
                    $"Slow endpoint: {context.Request.Method} {context.Request.Path} took {stopwatch.ElapsedMilliseconds} ms",
                    category: "performance",
                    level: BreadcrumbLevel.Warning);
            }
        }
    }
}
