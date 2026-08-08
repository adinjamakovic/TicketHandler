using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Geography.Countries.Queries.List
{
    public class ListCountriesQueryDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        // ISO 3166-1 alpha-2 code, e.g. "BA"
        public required string IsoCode { get; set; }
        public required string PhoneCode { get; set; }
        public string? Flag { get; set; }
    }
}
