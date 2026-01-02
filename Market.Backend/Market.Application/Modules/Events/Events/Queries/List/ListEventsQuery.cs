using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Queries.List
{
    public class ListEventsQuery : BasePagedQuery<ListEventsQueryDto>
    {
        //Search over name
        public string? Search {  get; set; }
    }
}
