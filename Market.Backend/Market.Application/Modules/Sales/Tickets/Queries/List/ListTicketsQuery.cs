using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Queries.List
{
    public class ListTicketsQuery : BasePagedQuery<ListTicketsQueryDto>
    {
        public string? EventName {  get; set; }
    }
}
