using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.TicketTypes.Commands.Create
{
    public class CreateTicketTypesCommand :IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
