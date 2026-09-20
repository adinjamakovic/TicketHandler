using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Cart.Commands.SaveForLater
{
    public class SaveCartItemForLaterCommand : IRequest<Unit>
    {
        public int TicketId { get; set; }
    }
}
