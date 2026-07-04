namespace Market.Application.Modules.Events.Events.Queries.GetCities;

public sealed class GetEventCitiesQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetEventCitiesQuery, List<string>>
{
    public async Task<List<string>> Handle(GetEventCitiesQuery req, CancellationToken ct)
    {
        return await ctx.Events
            .AsNoTracking()
            .Where(x => x.Venue.Location.City.Name != null)
            .Select(x => x.Venue.Location.City.Name)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);
    }
}
