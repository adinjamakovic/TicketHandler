using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Commands.Delete
{
    public class DeleteEventNewsCommand : IRequest<Unit>
    {
        public required int Id { get; set; }
    }
}
