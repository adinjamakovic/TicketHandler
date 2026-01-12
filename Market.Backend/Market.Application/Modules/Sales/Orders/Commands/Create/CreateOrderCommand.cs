using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Commands.Create
{
    public class CreateOrderCommand : IRequest<int>
    {
        public int PersonId { get; set; }
        public List<CreateOrderCommandItem> OrderItems { get; set; }
    }

    public class CreateOrderCommandItem
    {
        public int TicketId { get; set; }
        public decimal Quantity { get; set; }
    }
}
