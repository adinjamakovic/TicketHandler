using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Queries.GetByUserId
{
    public sealed class GetOrganizerByUserIdQuery : IRequest<GetOrganizerByUserIdQueryDto>
    {
        public int UserId { get; set; }
    }
}
