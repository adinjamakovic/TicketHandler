using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Commands.Delete
{
    public class DeleteOrganizerCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
