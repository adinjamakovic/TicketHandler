using Market.Application.Modules.Sales.Payments.Commands.Confirm;
using Market.Application.Modules.Sales.Payments.Commands.CreateIntent;
using Market.Application.Modules.Sales.Payments.Commands.HandleWebhook;
using Market.Application.Modules.Sales.Payments.Queries.GetConfig;
using Market.Application.Modules.Sales.Payments.Queries.GetQuote;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentsController(ISender sender) : ApiControllerBase(sender)
{
    // Publishable key and currency the browser initialises Stripe.js with.
    [HttpGet("config")]
    [AllowAnonymous]
    public async Task<GetPaymentConfigQueryDto> Config(CancellationToken ct)
    {
        var dto = await SendTraced(new GetPaymentConfigQuery(), ct);
        return dto;
    }

    // Server-priced breakdown of the current cart — the amount that will be charged.
    [HttpGet("quote")]
    public async Task<GetPaymentQuoteQueryDto> Quote(CancellationToken ct)
    {
        var dto = await SendTraced(new GetPaymentQuoteQuery(), ct);
        return dto;
    }

    [HttpPost("intent")]
    public async Task<CreatePaymentIntentCommandDto> CreateIntent(
        CreatePaymentIntentCommand command,
        CancellationToken ct)
    {
        var dto = await SendTraced(command, ct);
        return dto;
    }

    [HttpPost("confirm")]
    public async Task<ConfirmPaymentCommandDto> Confirm(ConfirmPaymentCommand command, CancellationToken ct)
    {
        var dto = await SendTraced(command, ct);
        return dto;
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook(CancellationToken ct)
    {
        // Must be the byte-exact body: the signature is computed over the raw payload.
        using var reader = new StreamReader(Request.Body);
        string payload = await reader.ReadToEndAsync(ct);

        await SendTraced(
            new HandlePaymentWebhookCommand
            {
                Payload = payload,
                Signature = Request.Headers["Stripe-Signature"]
            },
            ct);

        return Ok();
    }
}
