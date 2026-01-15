using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Application.Modules.Identity.Person.Queries.GetById
{
    public class GetPersonByIdQuery : IRequest<GetPersonByIdQueryDto>
    {
        public int Id { get; set; }
    }
}
