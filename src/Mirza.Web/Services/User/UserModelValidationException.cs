using System;

namespace Mirza.Web.Services.User
{
    public class UserModelValidationException : ArgumentException
    {
        public UserModelValidationException(string message) : base(message)
        {
        }

        public UserModelValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UserModelValidationException()
        {
        }
    }
}
