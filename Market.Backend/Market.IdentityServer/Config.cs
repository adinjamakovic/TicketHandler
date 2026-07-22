using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Market.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope(name: "api1", displayName: "TicketHandlerAPI")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // Dummy Client added by IdentityServer template
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "ap1" }
            },
            // Client for Market.API
            new Client
            {
                ClientId = "web",
                ClientSecrets = { new Secret("N3k1SuperJakS3cret!".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { "https://localhost:5002/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "verification",
                "api1"
            }
            }
        };
}
