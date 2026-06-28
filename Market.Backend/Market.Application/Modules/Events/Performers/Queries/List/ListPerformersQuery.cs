using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Performers.Queries.List
{
    public class ListPerformersQuery : BasePagedQuery<ListPerformersQueryDto>
    {
        //Search over name
        public string? Search {  get; set; }
    }
}
