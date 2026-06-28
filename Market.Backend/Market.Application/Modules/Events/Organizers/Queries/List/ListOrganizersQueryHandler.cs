namespace Market.Application.Modules.Events.Organizers.Queries.List;

public sealed class ListOrganizersQueryHandler(IAppDbContext ctx, IImageStorage imageStorage)
    : IRequestHandler<ListOrganizersQuery, PageResult<ListOrganizersQueryDto>>
{
    public async Task<PageResult<ListOrganizersQueryDto>> Handle(ListOrganizersQuery req, CancellationToken ct)
    {
        var q = ctx.Organizers.AsNoTracking();

        var searchTerm = req.Search?.Trim().ToLower() ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(searchTerm))
            q = q.Where(x => x.Name.ToLower().Contains(searchTerm));

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
