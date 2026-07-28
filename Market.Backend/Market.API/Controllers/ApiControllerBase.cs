using Market.API.Observability;
using System.Runtime.CompilerServices;

namespace Market.API.Controllers;
public abstract class ApiControllerBase(ISender sender) : ControllerBase
{
    protected ISender Sender { get; } = sender;
    protected Task<TResponse> SendTraced<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct,
        [CallerMemberName] string action = "")
        => SentryTracing.TraceAsync(
            SentryTracing.OperationFor(request),
            $"{GetType().Name}.{action} -> {request.GetType().Name}",
            () => Sender.Send(request, ct));
}
