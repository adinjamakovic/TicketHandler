using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

namespace Market.IdentityServer.Services;
public sealed class PersonProfileService(PersonCredentialStore store) : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context, CancellationToken ct = default)
    {
        var person = await store.FindBySubjectIdAsync(context.Subject.GetSubjectId(), ct);
        if (person is null)
            return;

        var claims = new List<Claim>
        {
            new("email", person.Email ?? string.Empty),
            new("is_admin", person.IsAdmin ? "true" : "false"),
            new("is_organiser", person.IsOrganiser ? "true" : "false"),
            new("is_user", person.IsUser ? "true" : "false")
        };

        var fullName = $"{person.FirstName} {person.LastName}".Trim();
        if (!string.IsNullOrWhiteSpace(fullName))
            claims.Add(new Claim("name", fullName));
            
        context.IssuedClaims.AddRange(claims);
    }

    public async Task IsActiveAsync(IsActiveContext context, CancellationToken ct = default)
    {
        var person = await store.FindBySubjectIdAsync(context.Subject.GetSubjectId(), ct);
        context.IsActive = person is { IsEnabled: true, IsDeleted: false };
    }
}
