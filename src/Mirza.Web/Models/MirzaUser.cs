using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Mirza.Web.Models
{
    public class MirzaUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; } = true;

        public int TeamId { get; set; }
        public Team Team { get; set; }

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public ICollection<AccessKey> AccessKeys { get; set; } = new List<AccessKey>();

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public ICollection<WorkLog> WorkLog { get; set; } = new List<WorkLog>();
    }
}