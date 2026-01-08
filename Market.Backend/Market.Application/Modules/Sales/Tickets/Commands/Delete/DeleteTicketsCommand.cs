using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Tickets.Commands.Delete
{
    public class DeleteTicketsCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
