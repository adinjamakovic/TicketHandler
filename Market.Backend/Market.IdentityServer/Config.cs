using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Market.IdentityServer;

public static class Config
{
    // The scope that represents access to the Market.API resource.
    public const string ApiScopeName = "market.api";

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
            new ApiScope(ApiScopeName, "TicketHandler API access")
        };
    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource(ApiScopeName, "TicketHandler API")
            {
                Scopes = { ApiScopeName },
                UserClaims =
                {
                    "email",
                    "name",
                    "is_admin",
                    "is_organiser",
                    "is_user"
                }
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // Swagger UI (Market.API)
            new Client
            {
                ClientId = "swagger",
                ClientName = "Market API - Swagger UI",

                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,   // public client, secured by PKCE
                RequireConsent = false,
                AllowAccessTokensViaBrowser = true,

                RedirectUris =
                {
                    "https://localhost:7260/swagger/oauth2-redirect.html",
                    "http://localhost:5177/swagger/oauth2-redirect.html"
                },
                PostLogoutRedirectUris =
                {
                    "https://localhost:7260/swagger/index.html",
                    "http://localhost:5177/swagger/index.html"
                },
                AllowedCorsOrigins =
                {
                    "https://localhost:7260",
                    "http://localhost:5177"
                },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    ApiScopeName
                },

                AccessTokenLifetime = 3600
            },

            // Angular SPA (Market.Frontend) 
            new Client
            {
                ClientId = "market.spa",
                ClientName = "TicketHandler Angular SPA",

                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                RequireConsent = false,
                AllowAccessTokensViaBrowser = true,

                RedirectUris = { "http://localhost:4200" },
                PostLogoutRedirectUris = { "http://localhost:4200" },
                AllowedCorsOrigins = { "http://localhost:4200" },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    ApiScopeName
                }
            }
        };
}
