namespace Market.Shared.Options;
public sealed class OpenAiOptions
{
    public const string SectionName = "OpenAi";

    public string BaseUrl { get; init; } = "https://api.openai.com/v1/";

    public string ApiKey { get; init; } = string.Empty;

    public string Model { get; init; } = "gpt-5.4-mini";

    public string SystemPrompt { get; init; } = string.Empty;

    public int TimeoutSeconds { get; init; } = 30;
}
