using Market.Application.Modules.Identity.Person.Commands.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Commands.Create
{
    public class CreateOrganizerCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        //TODO: Implement image upload
        //public byte[] Logo { get; set; } = Array.Empty<byte>();
        public CreateOrganizerCommandUser User {  get; set; }
    }

    public class CreateOrganizerCommandUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
    }
}
