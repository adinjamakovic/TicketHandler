using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Events.Performers.Queries.List
{
    public sealed class ListPerformersQueryDto
    {
        public int Id { get; set; }
        public string Name {  get; set; }
        public string? Description { get; set; }
        public string Genre { get; set; }
    }
}
