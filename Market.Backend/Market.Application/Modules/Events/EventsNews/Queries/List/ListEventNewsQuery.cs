using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Queries.List
{
    public class ListEventNewsQuery : BasePagedQuery<ListEventNewsQueryDto>
    {
        public int? OrganizerId {  get; set; }
        public int? EventId { get; set; }
    }
}
