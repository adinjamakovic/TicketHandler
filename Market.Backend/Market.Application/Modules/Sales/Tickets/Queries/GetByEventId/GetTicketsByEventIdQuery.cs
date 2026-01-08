using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Queries.GetByEventId
{
    public sealed class GetTicketsByEventIdQuery : BasePagedQuery<GetTicketsByEventIdQueryDto>
    {
        //Search over Event Id
        public int Id { get; init; } = -1;
    }
}
