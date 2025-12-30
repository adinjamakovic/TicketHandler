using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Queries.List
{
    public sealed class ListOrganizersQuery : BasePagedQuery<ListOrganizersQueryDto>
    {
        public string? Search { get; set; }
    }
}
