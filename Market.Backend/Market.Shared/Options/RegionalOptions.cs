namespace Market.Shared.Options;

public sealed class RegionalOptions
{
    public const string SectionName = "Regional";

    public string DefaultCulture { get; init; } = "bs-BA";

    public IReadOnlyList<string> SupportedCultures { get; init; } = ["bs-BA", "en-US"];

    public string QueryStringKey { get; init; } = "culture";
}
