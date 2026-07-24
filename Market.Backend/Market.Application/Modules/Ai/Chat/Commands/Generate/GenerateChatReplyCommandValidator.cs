namespace Market.Application.Modules.Ai.Chat.Commands.Generate
{
    public class GenerateChatReplyCommandValidator : AbstractValidator<GenerateChatReplyCommand>
    {
        public const int MaxPromptLength = 2000;

        public GenerateChatReplyCommandValidator()
        {
            RuleFor(x => x.Prompt)
                .NotEmpty().WithMessage("Prompt is required.")
                .MaximumLength(MaxPromptLength)
                .WithMessage($"Prompt must be {MaxPromptLength} characters or fewer.");
        }
    }
}
