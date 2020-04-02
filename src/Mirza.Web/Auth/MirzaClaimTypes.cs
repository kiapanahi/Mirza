using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mirza.Web.Auth
{
    public static class MirzaClaimTypes
    {
        public const string Teams = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/teamids";

        public const string Tenant = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/tenantid";
    }
}
