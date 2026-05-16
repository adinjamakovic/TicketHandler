using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Geography.Cities.Queries.List
{
    public class ListCitiesQueryDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string PostalCode { get; set; }
    }
}
