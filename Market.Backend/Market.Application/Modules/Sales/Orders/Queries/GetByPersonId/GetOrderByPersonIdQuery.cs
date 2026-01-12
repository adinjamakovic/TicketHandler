using Market.Application.Modules.Events.Events.Queries.GetById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.GetByPersonId
{
    public sealed class GetOrderByPersonIdQuery : BasePagedQuery<GetOrderByPersonIdQueryDto>
    {
        public int PersonId { get; set; }
    }
}
