using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.TicketTypes.Commands.Queries.GetById
{
    public class GetTicketTypesByIdQueryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
