using System;
using System.Collections.Generic;
using System.Linq;

namespace Mirza.Web.Services.User
{
    public class WorkLogModelValidationException : ArgumentException
    {
        public List<string> ValidationErrors { get; }

        public WorkLogModelValidationException(string[] validationErrorMessages, string message) : base(message)
        {
            ValidationErrors = validationErrorMessages.ToList();
        }

        public WorkLogModelValidationException(string message) : base(message)
        {
        }

        public WorkLogModelValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public WorkLogModelValidationException()
        {
        }

        public WorkLogModelValidationException(string[] validationErrorMessages)
        {
            ValidationErrors = validationErrorMessages.ToList();
        }
    }
}
