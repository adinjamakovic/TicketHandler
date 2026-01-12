using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Commands.Delete
{
    public class DeleteOrderCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
