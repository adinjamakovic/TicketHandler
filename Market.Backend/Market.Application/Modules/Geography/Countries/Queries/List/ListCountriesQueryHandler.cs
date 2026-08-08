using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Geography.Countries.Queries.List
{
    public sealed class ListCountriesQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListCountriesQuery, PageResult<ListCountriesQueryDto>>
    {
        public async Task<PageResult<ListCountriesQueryDto>> Handle(ListCountriesQuery req, CancellationToken ct)
        {
            var q = ctx.Countries.AsNoTracking();

            var searchTerm = req.Search?.Trim().ToLower() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(searchTerm))
                q = q.Where(x => x.Name.ToLower().Contains(searchTerm));

            var projectedQ = q.OrderBy(x => x.Name)
                .Select(x => new ListCountriesQueryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsoCode = x.IsoCode,
                    PhoneCode = x.PhoneCode,
                    Flag = x.Flag,
                });

            return await PageResult<ListCountriesQueryDto>.FromQueryableAsync(projectedQ, req.Paging, ct);
        }
    }
}
