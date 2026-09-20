using Market.Application.Abstractions.Payments;

namespace Market.Application.Modules.Sales.Payments.Commands.Confirm
{
    public sealed class ConfirmPaymentCommandDto
    {
        public required int OrderId { get; init; }
        public required string PaymentIntentId { get; init; }
        // Where the payment attempt itself stands at the provider
        public required PaymentIntentStatus PaymentStatus { get; init; }
        // Where the order stands in our own system.
        public required OrderStatusType OrderStatus { get; init; }
        // True once the order is paid for and reserved.
        public required bool IsPaid { get; init; }
        // True when the money went through but the order could not be completed and is now
        // with our team. The buyer has been charged, so this is not something to pay again.
        public required bool RequiresReview { get; init; }
        public required decimal Amount { get; init; }
        public required string Currency { get; init; }
        // Reason the last attempt failed, when the provider gave one.
        public string? FailureMessage { get; init; }
    }
}
