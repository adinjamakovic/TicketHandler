namespace Market.Application.Modules.Events.Events.Queries.GetByOrganizerId;

public sealed class GetEventsByOrganizerIdQuery : BasePagedQuery<GetEventsByOrganizerIdQueryDto>
{
    public int Id { get; init; }
}
