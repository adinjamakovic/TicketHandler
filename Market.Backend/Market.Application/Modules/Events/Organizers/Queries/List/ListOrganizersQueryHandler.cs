namespace Market.Application.Modules.Events.Organizers.Queries.List;

public sealed class ListOrganizersQueryHandler(IAppDbContext ctx, IImageStorage imageStorage)
    : IRequestHandler<ListOrganizersQuery, PageResult<ListOrganizersQueryDto>>
{
    public async Task<PageResult<ListOrganizersQueryDto>> Handle(ListOrganizersQuery req, CancellationToken ct)
    {
        var q = ctx.Organizers.AsNoTracking();

        var searchTerm = req.Search?.Trim().ToLower() ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(searchTerm))
            q = q.Where(x => x.Name.ToLower().Contains(searchTerm)
                          || (x.Description != null && x.Description.ToLower().Contains(searchTerm)));

        if (!string.IsNullOrWhiteSpace(req.City))
        {
            var city = req.City.Trim().ToLower();
            q = q.Where(x => x.City.Name.ToLower() == city);
        }

        if (!string.IsNullOrWhiteSpace(req.Email))
        {
            var email = req.Email.Trim().ToLower();
            q = q.Where(x => x.User.Email.ToLower().Contains(email));
        }

        if (req.HasEvents.HasValue)
            q = req.HasEvents.Value
                ? q.Where(x => x.Events.Any())
                : q.Where(x => !x.Events.Any());

        if (req.RegisteredFrom.HasValue)
        {
            var from = req.RegisteredFrom.Value.Date;
            q = q.Where(x => x.CreatedAtUtc.Date >= from);
        }

        if (req.RegisteredTo.HasValue)
        {
            var to = req.RegisteredTo.Value.Date;
            q = q.Where(x => x.CreatedAtUtc.Date <= to);
        }

        var projectedQuery = q.OrderBy(x => x.Name)
            .Select(x => new ListOrganizersQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                CityName = x.City.Name,
                UserName = x.User.UserName,
                EmailAddress = x.User.Email,
                Logo = x.Logo,
                IsDeleted = x.IsDeleted
            });

        var result = await PageResult<ListOrganizersQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);

        result.Items.ApplyPublicImagePaths(
            imageStorage,
            ImageStorageCategory.Organizers,
            x => x.Logo,
            (x, path) => x.Logo = path);

        return result;
    }
}
