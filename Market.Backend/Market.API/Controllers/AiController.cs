using Market.Application.Modules.Ai.Chat.Commands.Generate;
using Microsoft.AspNetCore.RateLimiting;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AiController(ISender sender) : ControllerBase
{
    [HttpPost("chat")]
    [EnableRateLimiting(DependencyInjection.AiRateLimitPolicy)]
    public async Task<GenerateChatReplyCommandDto> Chat(GenerateChatReplyCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        return result;
    }
}
