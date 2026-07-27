using MediatR;
using Sentry;

namespace Market.API.Observability;
public static class SentryTracing
{
    public const string CommandOperation = "mediatr.command";
    public const string QueryOperation = "mediatr.query";
    public static async Task<T> TraceAsync<T>(string operation, string description, Func<Task<T>> work)
    {
        var parent = SentrySdk.GetSpan();
        if (parent is null)
            return await work();

        var span = parent.StartChild(operation, description);
        try
        {
            var result = await work();
            span.Finish(SpanStatus.Ok);
            return result;
        }
        catch (Exception ex)
        {
            span.Finish(ex);
            throw;
        }
    }

    public static async Task TraceAsync(string operation, string description, Func<Task> work)
    {
        await TraceAsync(operation, description, async () =>
        {
            await work();
            return true;
        });
    }
    public static string OperationFor(object request) =>
        request.GetType().Name.EndsWith("Command", StringComparison.Ordinal)
            ? CommandOperation
            : QueryOperation;
}
