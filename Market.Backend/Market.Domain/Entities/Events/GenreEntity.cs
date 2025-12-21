using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Events
{
    /// <summary>
    /// Identifies a genre
    /// </summary>
    public class GenreEntity : BaseEntity
    {
        /// <summary>
        /// Genre name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Genre description
        /// </summary>
        public string Description { get; set; }
        public ICollection<PerformerEntity> Performers { get; set; } = new List<PerformerEntity>();
    }
}
