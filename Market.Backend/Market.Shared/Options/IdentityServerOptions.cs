namespace Market.Shared.Options;
public sealed class IdentityServerOptions
{
    public const string SectionName = "IdentityServer";
    public string Authority { get; init; } = "https://localhost:5001";
    public string Audience { get; init; } = "market.api";
    public bool RequireHttpsMetadata { get; init; } = true;
}
