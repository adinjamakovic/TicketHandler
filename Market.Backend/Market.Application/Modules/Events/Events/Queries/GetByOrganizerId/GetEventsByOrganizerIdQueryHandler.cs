namespace Market.Application.Modules.Events.Events.Queries.GetByOrganizerId;

public sealed class GetEventsByOrganizerIdQueryHandler(IAppDbContext ctx, IImageStorage imageStorage)
    : IRequestHandler<GetEventsByOrganizerIdQuery, PageResult<GetEventsByOrganizerIdQueryDto>>
{
    public async Task<PageResult<GetEventsByOrganizerIdQueryDto>> Handle(GetEventsByOrganizerIdQuery req, CancellationToken ct)
    {
        var q = ctx.Events.AsNoTracking().Where(x => x.OrganizerId == req.Id);

        var projectedQuery = q.OrderBy(x => x.ScheduledDate)
            .Select(x => new GetEventsByOrganizerIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ScheduledDate = x.ScheduledDate,
                OrganizerName = x.Organizer.Name,
                VenueName = x.Venue.Name,
                Image = x.Image,
                EventTypeName = x.EventType.Name,
                Performers = x.PerformerEvents
                    .Select(y => new GetEventsByOrganizerIdQueryDtoPerformers
                    {
                        Name = y.Performer.Name,
                        Description = y.Performer.Description,
                        Genre = y.Performer.Genre.Name
                    }).ToList()
            });

        var result = await PageResult<GetEventsByOrganizerIdQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);

        result.Items.ApplyPublicImagePaths(
            imageStorage,
            ImageStorageCategory.Events,
            x => x.Image,
            (x, path) => x.Image = path);

        return result;
    }
}
