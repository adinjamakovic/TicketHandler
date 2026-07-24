using Market.Application.Abstractions;
using Market.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Market.Infrastructure.Common;
public sealed class OpenAiCompletionService(
    HttpClient http,
    IOptions<OpenAiOptions> options,
    ILogger<OpenAiCompletionService> logger) : IAiCompletionService
{
    private readonly OpenAiOptions _options = options.Value;

    public async Task<string> CompleteAsync(string prompt, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException(
                $"{OpenAiOptions.SectionName}:ApiKey is not configured. Set it via user-secrets or an environment variable.");

        var messages = new List<ChatMessage>();

        if (!string.IsNullOrWhiteSpace(_options.SystemPrompt))
            messages.Add(new ChatMessage { Role = "system", Content = _options.SystemPrompt });

        messages.Add(new ChatMessage { Role = "user", Content = prompt });

        using var response = await http.PostAsJsonAsync(
            "chat/completions",
            new ChatCompletionRequest { Model = _options.Model, Messages = messages },
            ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);

            logger.LogError(
                "OpenAI chat completion failed. Status: {StatusCode}, Body: {Body}",
                (int)response.StatusCode,
                body);

            throw new HttpRequestException(
                $"OpenAI chat completion failed with status {(int)response.StatusCode}.");
        }

        var completion = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(ct);
        var reply = completion?.Choices?.FirstOrDefault()?.Message?.Content;

        if (string.IsNullOrWhiteSpace(reply))
            throw new HttpRequestException("OpenAI returned an empty chat completion.");

        return reply.Trim();
    }

    private sealed class ChatCompletionRequest
    {
        [JsonPropertyName("model")]
        public required string Model { get; init; }

        [JsonPropertyName("messages")]
        public required IReadOnlyList<ChatMessage> Messages { get; init; }
    }

    private sealed class ChatMessage
    {
        [JsonPropertyName("role")]
        public required string Role { get; init; }

        [JsonPropertyName("content")]
        public required string Content { get; init; }
    }

    private sealed class ChatCompletionResponse
    {
        [JsonPropertyName("choices")]
        public List<ChatCompletionChoice>? Choices { get; init; }
    }

    private sealed class ChatCompletionChoice
    {
        [JsonPropertyName("message")]
        public ChatCompletionMessage? Message { get; init; }
    }

    private sealed class ChatCompletionMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; init; }
    }
}
