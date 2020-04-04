using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mirza.Web.Services.User
{
    public class AccessKeyException: ArgumentException
    {
        public AccessKeyException(string message) : base(message)
        {
        }

        public AccessKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public AccessKeyException()
        {
        }
    }
}
