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
        public required int CityId { get; set; }
        public required string City { get; set; }
        public required string Address { get; set; }
        public required GetOrganizerByIdQueryDtoUser User { get; set; }
        public List<GetOrganizerByIdQueryEventDto> Events { get; set; }
    }

    public class GetOrganizerByIdQueryDtoUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class GetOrganizerByIdQueryEventDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string? Description { get; set; }
        public required DateTime ScheduledDate { get; set; }
    }
}
