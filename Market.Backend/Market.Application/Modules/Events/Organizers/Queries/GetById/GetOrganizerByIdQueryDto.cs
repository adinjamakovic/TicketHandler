using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Queries.GetById
{
    public class GetOrganizerByIdQueryDto 
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string? Description { get; set; }
        public required string Email { get; set; }
        public required string City { get; set; }
        public List<GetOrganizerByIdQueryEventDto> Events { get; set; }
    }
}
