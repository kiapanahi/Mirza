using System;
using System.Collections.Generic;
using System.Linq;

namespace Mirza.Web.Services.User
{
    public class UserModelValidationException : ArgumentException
    {
        public List<string> ValidationErrors { get; }

        public UserModelValidationException(string[] validationErrorMessages, string message) : base(message)
        {
            ValidationErrors = validationErrorMessages.ToList();
        }

        public UserModelValidationException(string message) : base(message)
        {
        }

        public UserModelValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UserModelValidationException()
        {
        }

        public UserModelValidationException(string[] validationErrorMessages)
        {
            ValidationErrors = validationErrorMessages.ToList();
        }
    }
}
