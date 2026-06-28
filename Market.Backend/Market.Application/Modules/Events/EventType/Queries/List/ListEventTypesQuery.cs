using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventType.Queries.List
{
    public class ListEventTypesQuery : BasePagedQuery<ListEventTypesQueryDto>
    {
        public string? Search {  get; set; }
    }
}
