namespace Market.Application.Modules.Ai.Chat.Commands.Generate
{
    public class GenerateChatReplyCommand : IRequest<GenerateChatReplyCommandDto>
    {
        public required string Prompt { get; set; }
    }
}
