using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Geography.Cities.Queries.List
{
    public class ListCitiesQuery : BasePagedQuery<ListCitiesQueryDto>
    {
        //Search over city name
        public string? Search {  get; set; }
    }
}
