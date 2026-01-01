using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Queries.List
{
    public sealed class ListOrganizersQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListOrganizersQuery, PageResult<ListOrganizersQueryDto>>
    {
        public async Task<PageResult<ListOrganizersQueryDto>> Handle(ListOrganizersQuery req, CancellationToken ct)
        {
            var q = ctx.Organizers.AsNoTracking();

            var searchTerm = req.Search?.Trim().ToLower() ?? string.Empty;

            if(!string.IsNullOrWhiteSpace(searchTerm))
            {
                q = q.Where(x => x.Name.ToLower().Contains(searchTerm));
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
                    IsDeleted = x.IsDeleted
                });

            return await PageResult<ListOrganizersQueryDto>.FromQueryableAsync(projectedQuery, req.Paging, ct);
        }
    }
}
