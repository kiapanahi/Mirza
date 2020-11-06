using System.Collections.Generic;

namespace Mirza.Web.Models
{
    public class MirzaTenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public ICollection<MirzaTeam> Teams { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public ICollection<MirzaUser> Members { get; set; }
    }
}
