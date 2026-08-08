using Market.Application.Abstractions.Payments;
using Market.Application.Modules.Sales.Cart;

namespace Market.Application.Modules.Sales.Payments.Queries.GetQuote
{
    public sealed class GetPaymentQuoteQueryHandler(
        IAppDbContext ctx,
        IAppCurrentUser appCurrentUser,
        IPaymentGateway gateway)
        : IRequestHandler<GetPaymentQuoteQuery, GetPaymentQuoteQueryDto>
    {
        public async Task<GetPaymentQuoteQueryDto> Handle(GetPaymentQuoteQuery req, CancellationToken ct)
        {
            int personId = CartPerson.RequireId(appCurrentUser);

            var basket = await CheckoutBasket.BuildAsync(ctx, personId, ct);

            return new GetPaymentQuoteQueryDto
            {
                Currency = gateway.Currency,
                LineCount = basket.Lines.Count,
                TicketCount = basket.TicketCount,
                Subtotal = basket.Subtotal,
                DiscountAmount = basket.DiscountAmount,
                Total = basket.Total,
                Lines = basket.Lines.Select(x => new GetPaymentQuoteQueryDtoLine
                {
                    TicketId = x.TicketId,
                    EventName = x.EventName,
                    TicketTypeName = x.TicketTypeName,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Subtotal = x.Subtotal,
                    DiscountAmount = x.DiscountAmount,
                    Total = x.Total
                }).ToList()
            };
        }
    }
}
