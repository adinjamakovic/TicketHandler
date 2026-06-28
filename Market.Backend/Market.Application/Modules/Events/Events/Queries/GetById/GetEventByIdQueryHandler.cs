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
                VenueId = x.VenueId,
                Image = x.Image,
                EventTypeId = x.EventTypeId,
                Performers = x.PerformerEvents
                    .Select(y => new GetEventByIdQueryDtoPerformers
                    {
                        PerformerId = y.Performer.Id,
                        TimeStamp = y.TimeStamp
                    }).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            throw new MarketNotFoundException($"Event with Id {req.Id} not found");

        dto.Image = imageStorage.ToPublicPath(ImageStorageCategory.Events, dto.Image);
        return dto;
    }
}
