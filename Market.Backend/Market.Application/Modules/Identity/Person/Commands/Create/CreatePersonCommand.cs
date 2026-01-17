using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Identity.Person.Commands.Create
{
    public class CreatePersonCommand: IRequest<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Phone {  get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password{ get; set; }
        public bool IsAdmin { get; set; }
        public bool isOrganiser {  get; set; }
        public bool isUser {  get; set; }
    }
}
