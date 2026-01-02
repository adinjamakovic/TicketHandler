using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Queries.GetById
{
    public sealed class GetEventByIdQuery : IRequest<GetEventByIdQueryDto>
    {
        public int Id { get; set; }
    }
}
