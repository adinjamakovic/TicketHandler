using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Queries.GetByEventName
{
    public sealed class GetTicketsByEventNameQuery : BasePagedQuery<GetTicketsByEventNameQueryDto>
    {
        public required string EventName { get; set; }
    }
}
