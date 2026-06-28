using Microsoft.AspNetCore.Http;

namespace Market.Application.Modules.Events.Organizers.Commands.Update;

public class UpdateOrganizerCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Address { get; set; } = string.Empty;
    public int CityId { get; set; }
    public IFormFile? Logo { get; set; }
    public UpdateOrganizerCommandUser User { get; set; } = null!;
}

public class UpdateOrganizerCommandUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int CityId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
