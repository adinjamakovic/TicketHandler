using Market.Application.Modules.Events.Events.Queries.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Events.Queries.GetById
{
    public sealed class GetEventByIdQueryDto
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required string? Description { get; init; }
        public required DateTime ScheduledDate { get; init; }
        public required string OrganizerName { get; init; }
        public required string VenueName { get; init; }
        public required byte[]? Image { get; init; }
        public required string EventTypeName { get; init; }
        public required List<GetEventByIdQueryDtoPerformers> Performers { get; init; }
    }

    public sealed class GetEventByIdQueryDtoPerformers
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public required string? Description { get; init; }
        public required string Genre { get; init; }
    }
}
