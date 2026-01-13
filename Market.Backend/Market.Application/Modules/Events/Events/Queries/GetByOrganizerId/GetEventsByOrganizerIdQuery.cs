using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Queries.List
{
    public sealed class GetEventsByOrganizerIdQuery : BasePagedQuery<GetEventsByOrganizerIdQueryDto>
    {
        //Search over Organizer Id
        public int Id { get; init; } = 1;
    }
}
