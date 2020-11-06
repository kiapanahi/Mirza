using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mirza.Web.Models
{
    public class TeamUser
    {
        public int TeamId { get; set; }
        public MirzaTeam Team { get; set; }
        public int UserId { get; set; }
        public MirzaUser User { get; set; }
    }
}
