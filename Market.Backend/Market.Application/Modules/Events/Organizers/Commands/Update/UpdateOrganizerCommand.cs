using Market.Application.Modules.Events.Organizers.Commands.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Commands.Update
{
    public class UpdateOrganizerCommand : IRequest<Unit>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        //Figure out how to implement picture updating
        //public byte[] Logo { get; set; }
        //Not sure if this is needed yet, will edit as the project goes on
        public UpdateOrganizerCommandUser User { get; set; }
    }

    public class UpdateOrganizerCommandUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
