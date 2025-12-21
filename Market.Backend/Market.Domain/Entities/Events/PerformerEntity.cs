using Market.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Domain.Entities.Events
{

    /// <summary>
    /// Identifies a performer
    /// </summary>
    public class PerformerEntity : BaseEntity
    {
        /// <summary>
        /// Performer stage name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Performer image
        /// </summary>
        public byte[] Image { get; set; }
        /// <summary>
        /// Performer decription
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Identifier of the genre the performe is a part of
        /// </summary>
        public int GenreId { get; set; }
        /// <summary>
        /// Genre that the performer is a part of
        /// </summary>
        public GenreEntity Genre { get; set; }
        public ICollection<PerformerEventEntity> PerformerEvents { get; set; } = new List<PerformerEventEntity>();
    }
}
