using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Mirza.Web.Models
{
    public class MirzaTeam
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int TenantId { get; set; }
        public MirzaTenant Tenant { get; set; }

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public ICollection<TeamUser> Members { get; set; }
    }
}