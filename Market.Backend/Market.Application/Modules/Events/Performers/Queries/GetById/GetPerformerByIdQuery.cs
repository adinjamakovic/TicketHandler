using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Performers.Queries.GetById
{
    public sealed class GetPerformerByIdQuery : IRequest<GetPerformerByIdQueryDto>
    {
        public int Id { get; set; }
    }
}
