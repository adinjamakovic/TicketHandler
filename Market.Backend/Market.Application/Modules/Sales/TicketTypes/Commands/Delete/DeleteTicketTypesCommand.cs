using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.TicketTypes.Commands.Delete
{
    public class DeleteTicketTypesCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
