using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Queries.List
{
    public sealed class ListEventsQueryDto
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required string? Description { get; init; }
        public required DateTime ScheduledDate { get; init; }
        public required ListEventsQueryDtoOrganizer Organizer { get; init; }
        public required byte[] Image { get; init; }
        public required string VenueName { get; init; }
        public required string EventType { get; init; }
    }

    public sealed class ListEventsQueryDtoOrganizer
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required string Address { get; init; }
        public required string City { get; init; }
        public required string UserName { get; init; }
    }
}
