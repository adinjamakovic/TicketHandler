using Market.Application.Abstractions.Payments;

namespace Market.Application.Modules.Sales.Payments.Queries.GetConfig
{
    public sealed class GetPaymentConfigQueryHandler(IPaymentGateway gateway)
        : IRequestHandler<GetPaymentConfigQuery, GetPaymentConfigQueryDto>
    {
        public Task<GetPaymentConfigQueryDto> Handle(GetPaymentConfigQuery req, CancellationToken ct) =>
            Task.FromResult(new GetPaymentConfigQueryDto
            {
                PublishableKey = gateway.PublishableKey,
                Currency = gateway.Currency,
                IsConfigured = gateway.IsConfigured
            });
    }
}
