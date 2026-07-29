using Market.Application.Abstractions;

namespace Market.Tests.Common;

/// <summary>
/// Stand-in for <see cref="IAppCurrentUser"/> so handlers can be exercised without an HTTP context.
/// </summary>
public sealed class FakeAppCurrentUser : IAppCurrentUser
{
    public int? UserId { get; init; }
    public string? Email { get; init; }
    public bool IsAuthenticated { get; init; } = true;
    public bool IsAdmin { get; init; }
    public bool IsOrganiser { get; init; }
    public bool IsUser { get; init; }

    public static FakeAppCurrentUser Organiser(int userId) => new()
    {
        UserId = userId,
        Email = $"organiser{userId}@test.local",
        IsOrganiser = true
    };

    public static FakeAppCurrentUser Admin(int userId = 900) => new()
    {
        UserId = userId,
        Email = $"admin{userId}@test.local",
        IsAdmin = true
    };

    public static FakeAppCurrentUser User(int userId = 800) => new()
    {
        UserId = userId,
        Email = $"user{userId}@test.local",
        IsUser = true
    };

    public static FakeAppCurrentUser Anonymous() => new()
    {
        IsAuthenticated = false
    };
}
