namespace Market.Application.Modules.Ai.Chat.Commands.Generate
{
    public class GenerateChatReplyCommandHandler(IAiCompletionService aiCompletion)
        : IRequestHandler<GenerateChatReplyCommand, GenerateChatReplyCommandDto>
    {
        public async Task<GenerateChatReplyCommandDto> Handle(GenerateChatReplyCommand req, CancellationToken ct)
        {
            var reply = await aiCompletion.CompleteAsync(req.Prompt.Trim(), ct);

            return new GenerateChatReplyCommandDto { Reply = reply };
        }
    }
}
