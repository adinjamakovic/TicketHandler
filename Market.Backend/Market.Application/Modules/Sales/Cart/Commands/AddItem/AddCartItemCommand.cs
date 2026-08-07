using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Commands.AddItem
{
    public class AddCartItemCommand : IRequest<Unit>
    {
        public int TicketId { get; set; }
        public decimal Quantity { get; set; } = 1;
    }
}
