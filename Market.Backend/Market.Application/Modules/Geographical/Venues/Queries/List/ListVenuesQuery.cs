using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Geographical.Venues.Queries.List
{
    public class ListVenuesQuery : BasePagedQuery<ListVenuesQueryDto>
    {
        public string? Search {  get; set; }
    }
}
