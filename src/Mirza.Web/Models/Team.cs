using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Mirza.Web.Models
{
    public class Team
    {
        public Team()
        {
        }

        public Team(string name)
        {
            Name = name;
        }

        public Team(int id, string name) : this(name)
        {
            Id = id;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public ICollection<MirzaUser> Members { get; set; }
    }
}