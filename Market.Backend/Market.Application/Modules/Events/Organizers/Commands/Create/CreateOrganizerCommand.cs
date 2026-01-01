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
        public int UserId {  get; set; }
    }
}
