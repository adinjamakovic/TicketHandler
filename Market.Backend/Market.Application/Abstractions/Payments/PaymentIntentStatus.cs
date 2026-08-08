namespace Market.Application.Abstractions.Payments;

public enum PaymentIntentStatus
{
    RequiresPaymentMethod = 1,
    RequiresConfirmation = 2,
    RequiresAction = 3,
    Processing = 4,
    RequiresCapture = 5,
    Succeeded = 6,
    Canceled = 7
}
