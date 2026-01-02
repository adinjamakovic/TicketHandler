using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Queries.List
{
    public sealed class ListEventsWithPerformersQuery : BasePagedQuery<ListEventsWithPerformersQueryDto>
    {
        //Search over name
        public string? Search {  get; init; }
    }
}
