using Market.Infrastructure.Common;
using Market.Shared.Dtos;
using Market.Shared.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Market.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        // Controllers + uniform BadRequest
        services.AddControllers()
            .ConfigureApiBehaviorOptions(opts =>
            {
                opts.InvalidModelStateResponseFactory = ctx =>
                {
                    var msg = string.Join("; ",
                        ctx.ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                                                 ? "Validation error"
                                                 : e.ErrorMessage));
                    return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new ErrorDto
                    {
                        Code = "validation.failed",
                        Message = msg
                    });
                };
            });

        // Bearer auth: access tokens are issued and signed by Duende IdentityServer.
        var idsvr = configuration.GetSection(IdentityServerOptions.SectionName).Get<IdentityServerOptions>()
                    ?? new IdentityServerOptions();

        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.Authority = idsvr.Authority;              
            o.Audience = idsvr.Audience;                
            o.RequireHttpsMetadata = idsvr.RequireHttpsMetadata;
            o.TokenValidationParameters.ValidateAudience = true;
        });

        services.AddAuthorization(o =>
        {
            o.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        // Swagger with Bearer auth
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Market API", Version = "v1" });
            var xml = Path.Combine(AppContext.BaseDirectory, "Market.API.xml");
            if (File.Exists(xml))
                c.IncludeXmlComments(xml, includeControllerXmlComments: true);

            var authorizationUrl = new Uri($"{idsvr.Authority}/connect/authorize");
            var tokenUrl = new Uri($"{idsvr.Authority}/connect/token");

            var oauth = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "Log in through Duende IdentityServer (Authorization Code + PKCE).",
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = authorizationUrl,
                        TokenUrl = tokenUrl,
                        Scopes = new Dictionary<string, string>
                        {
                            ["openid"] = "Your user identifier",
                            ["profile"] = "Your profile information",
                            ["email"] = "Your email address",
                            [idsvr.Audience] = "Access to the Market API"
                        }
                    }
                },
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            };
            c.AddSecurityDefinition("oauth2", oauth);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { oauth, new[] { "openid", "profile", "email", idsvr.Audience } }
            });
        });

        services.AddExceptionHandler<MarketExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}