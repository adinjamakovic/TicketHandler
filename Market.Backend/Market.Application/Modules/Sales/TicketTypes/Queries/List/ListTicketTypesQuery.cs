using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.TicketTypes.Commands.Queries.List
{
    public sealed class ListTicketTypesQuery : BasePagedQuery<ListTicketTypesQueryDto>
    {
        public string? Search { get; set; }
    }
}
