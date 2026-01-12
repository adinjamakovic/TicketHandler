using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.ListWithTicketsAndEvents
{
    public class ListOrdersWithTicketsAndEventsQuery : BasePagedQuery<ListOrdersWithTicketsAndEventsQueryDto>
    {
        
        public int? Id{  get; set; }
    }
}
