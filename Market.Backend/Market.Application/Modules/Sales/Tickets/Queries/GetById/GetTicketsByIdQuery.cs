using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Queries.GetById
{
    public sealed class GetTicketsByIdQuery : IRequest<GetTicketsByIdQueryDto>
    {
        public int Id { get; set; }
    }
}
