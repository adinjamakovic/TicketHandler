using Market.Application.Modules.Sales.Cart;

namespace Market.Application.Modules.Sales.Payments.Commands.Confirm
{
    internal sealed class ConfirmPaymentCommandHandler(
        IAppCurrentUser appCurrentUser,
        PaymentSettlementService settlement)
        : IRequestHandler<ConfirmPaymentCommand, ConfirmPaymentCommandDto>
    {
        public async Task<ConfirmPaymentCommandDto> Handle(ConfirmPaymentCommand req, CancellationToken ct)
        {
            int personId = CartPerson.RequireId(appCurrentUser);

            var result = await settlement.SettleAsync(req.PaymentIntentId, personId, ct);

            return new ConfirmPaymentCommandDto
            {
                OrderId = result.OrderId,
                PaymentIntentId = result.PaymentIntentId,
                PaymentStatus = result.Status,
                OrderStatus = result.OrderStatus,
                IsPaid = result.IsPaid,
                Amount = result.Amount,
                Currency = result.Currency,
                FailureMessage = result.FailureMessage
            };
        }
    }
}
