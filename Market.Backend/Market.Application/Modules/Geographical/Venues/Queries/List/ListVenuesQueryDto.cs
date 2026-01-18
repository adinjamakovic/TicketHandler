using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Geographical.Venues.Queries.List
{
    public class ListVenuesQueryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Seated { get; set; }
        public int Standing { get; set; }
        public string Location { get; set; }
    }
}
