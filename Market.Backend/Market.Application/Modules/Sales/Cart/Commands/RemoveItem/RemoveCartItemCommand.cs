using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Commands.RemoveItem
{
    public class RemoveCartItemCommand : IRequest<Unit>
    {
        public required int TicketId { get; set; }
    }
}
