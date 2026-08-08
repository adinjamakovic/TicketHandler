using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Geography.Countries.Queries.List
{
    public class ListCountriesQuery : BasePagedQuery<ListCountriesQueryDto>
    {
        //Search over country name
        public string? Search { get; set; }
    }
}
