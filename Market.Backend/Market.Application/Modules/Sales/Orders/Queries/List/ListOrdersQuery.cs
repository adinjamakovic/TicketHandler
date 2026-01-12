using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.List
{
    public class ListOrdersQuery : BasePagedQuery<ListOrdersQueryDto>
    {
        public int? Id{  get; set; }
    }
}
