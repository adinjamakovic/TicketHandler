using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Queries.GetById
{
    public sealed class GetOrganizerByIdQuery : IRequest<GetOrganizerByIdQueryDto>
    {
        public int Id { get; set; }
    }
}
