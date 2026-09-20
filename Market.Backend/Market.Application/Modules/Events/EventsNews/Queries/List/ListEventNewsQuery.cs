using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.EventsNews.Queries.List
{
    public class ListEventNewsQuery : BasePagedQuery<ListEventNewsQueryDto>
    {
        /// <summary>Ignored for organiser callers — they are always scoped to their own organizer.</summary>
        public int? OrganizerId {  get; set; }
        /// <summary>Keeps only posts written for this event.</summary>
        public int? EventId { get; set; }
        /// <summary>Free text matched against the post header and body.</summary>
        public string? Search { get; set; }
        /// <summary>Lower bound (inclusive) of the date the post was published.</summary>
        public DateTime? DateFrom { get; set; }
        /// <summary>Upper bound (inclusive) of the date the post was published.</summary>
        public DateTime? DateTo { get; set; }
        /// <summary>True keeps only posts that carry an image, false only those without one.</summary>
        public bool? HasImage { get; set; }
    }
}
