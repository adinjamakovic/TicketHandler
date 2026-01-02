using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Commands.Delete
{
    public class DeleteEventCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
