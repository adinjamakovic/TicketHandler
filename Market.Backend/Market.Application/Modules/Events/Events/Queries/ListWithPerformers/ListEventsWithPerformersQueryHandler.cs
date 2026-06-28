namespace Market.Application.Modules.Events.Events.Queries.ListWithPerformers;

public sealed class ListEventsWithPerformersQueryHandler(IAppDbContext ctx, IImageStorage imageStorage)
    : IRequestHandler<ListEventsWithPerformersQuery, PageResult<ListEventsWithPerformersQueryDto>>
{
    public async Task<PageResult<ListEventsWithPerformersQueryDto>> Handle(ListEventsWithPerformersQuery req, CancellationToken ct)
    {
        var q = ctx.Events.AsNoTracking();

        var searchTerm = req.Search?.ToLower().Trim() ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(searchTerm))
            q = q.Where(x => x.Name.ToLower().Contains(searchTerm));

        var projectedQuery = q.OrderBy(x => x.ScheduledDate)
            .Select(x => new ListEventsWithPerformersQueryDto
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
                    .Select(y => new ListEventsWithPerformersQueryDtoPerformers
                    {
                        Name = y.Performer.Name,
                        Description = y.Performer.Description,
                        Genre = y.Performer.Genre.Name
                    }).ToList()
            });

        var result = await PageResult<ListEventsWithPerformersQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);

        result.Items.ApplyPublicImagePaths(
            imageStorage,
            ImageStorageCategory.Events,
            x => x.Image,
            (x, path) => x.Image = path);

        return result;
    }
}
