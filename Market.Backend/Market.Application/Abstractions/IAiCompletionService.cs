namespace Market.Application.Abstractions;
public interface IAiCompletionService
{
    Task<string> CompleteAsync(string prompt, CancellationToken ct = default);
}
