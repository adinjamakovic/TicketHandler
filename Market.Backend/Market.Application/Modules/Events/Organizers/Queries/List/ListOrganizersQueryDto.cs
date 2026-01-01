using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Organizers.Queries.List
{
    public sealed class ListOrganizersQueryDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string? Description { get; set; }
        public required string CityName { get; set; }
        public required string UserName { get; set; }
        public required string EmailAddress { get; set; }
        public required bool IsDeleted { get; set; }
    }
}
