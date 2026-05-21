namespace Market.Application.Modules.Events.Events.Queries.GetById;

public sealed class GetEventByIdQueryHandler(IAppDbContext ctx, IImageStorage imageStorage)
    : IRequestHandler<GetEventByIdQuery, GetEventByIdQueryDto>
{
    public async Task<GetEventByIdQueryDto> Handle(GetEventByIdQuery req, CancellationToken ct)
    {
        var dto = await ctx.Events
            .Where(x => x.Id == req.Id)
            .Select(x => new GetEventByIdQueryDto
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
                    .Select(y => new GetEventByIdQueryDtoPerformers
                    {
                        Id = y.Performer.Id,
                        Name = y.Performer.Name,
                        Description = y.Performer.Description,
                        Genre = y.Performer.Genre.Name,
                    }).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            throw new MarketNotFoundException($"Event with Id {req.Id} not found");

        dto.Image = imageStorage.ToPublicPath(ImageStorageCategory.Events, dto.Image);
        return dto;
    }
}
