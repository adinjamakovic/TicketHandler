using Market.Application.Modules.Events.Events.Queries.GetById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Sales.Orders.Queries.GetById
    {
    public sealed class GetOrderByIdQuery : IRequest<GetOrderByIdQueryDto>
    {
        public int Id { get; set; }
    }
}
