using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Commands.MoveToCart
{
    public class MoveCartItemToCartCommand : IRequest<Unit>
    {
        public int TicketId { get; set; }
    }
}
