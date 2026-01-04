using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Queries.GetById
{
    public class GetEventNewsByIdQuery : IRequest<GetEventNewsByIdQueryDto>
    {
        public int Id { get; set; }
    }
}
