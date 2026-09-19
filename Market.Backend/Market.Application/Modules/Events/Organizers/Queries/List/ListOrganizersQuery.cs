using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Queries.List
{
    public sealed class ListOrganizersQuery : BasePagedQuery<ListOrganizersQueryDto>
    {
        /// <summary>Free text matched against the organizer name and description.</summary>
        public string? Search { get; set; }
        /// <summary>Name of the city the organizer is registered in.</summary>
        public string? City { get; set; }
        /// <summary>Partial match against the e-mail of the organizer's user account.</summary>
        public string? Email { get; set; }
        /// <summary>True keeps only organizers that have at least one event, false only those without any.</summary>
        public bool? HasEvents { get; set; }
    }
}
