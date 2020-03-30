using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mirza.Web.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public ICollection<UserTeam> Members { get; set; }
    }
}
