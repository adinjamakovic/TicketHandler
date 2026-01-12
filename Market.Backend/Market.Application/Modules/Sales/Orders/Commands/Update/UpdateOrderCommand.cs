using Market.Application.Modules.Events.Events.Commands.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Sales.Sales.Orders.Commands.Update
{
    public class UpdateOrderCommand : IRequest<int>
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public List<UpdateOrderCommandItem> OrderItems { get; set; }
    }

    public class UpdateOrderCommandItem
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public decimal Quantity { get; set; }
    }
}
