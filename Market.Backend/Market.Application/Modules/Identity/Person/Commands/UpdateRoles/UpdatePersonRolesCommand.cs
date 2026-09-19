namespace Market.Application.Modules.Identity.Person.Commands.UpdateRoles;

public class UpdatePersonRolesCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsOrganiser { get; set; }
    public bool IsUser { get; set; }
}
