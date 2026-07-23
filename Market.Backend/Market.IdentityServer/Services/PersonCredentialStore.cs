using Market.Domain.Entities.Identity;
using Market.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Market.IdentityServer.Services;
public sealed class PersonCredentialStore(
    DatabaseContext db,
    IPasswordHasher<PersonEntity> hasher)
{
    public async Task<PersonEntity?> ValidateCredentialsAsync(
        string email, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var normalized = email.Trim().ToLowerInvariant();

        var person = await db.Persons
            .FirstOrDefaultAsync(
                p => p.Email.ToLower() == normalized && p.IsEnabled && !p.IsDeleted, ct);

        if (person is null)
            return null;

        var result = hasher.VerifyHashedPassword(person, person.PasswordHash, password);

        return result == PasswordVerificationResult.Failed ? null : person;
    }
    public Task<PersonEntity?> FindBySubjectIdAsync(
        string? subjectId, CancellationToken ct = default)
    {
        if (int.TryParse(subjectId, out var id))
            return db.Persons.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

        return Task.FromResult<PersonEntity?>(null);
    }
}
