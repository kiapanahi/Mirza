using System;

namespace Mirza.Web.Services.User
{
    public class DuplicateEmailException : ArgumentException
    {
        public string Email { get; }

        public DuplicateEmailException(string email, string message) : base(message)
        {
            Email = email;
        }

        public DuplicateEmailException(string email, string message, Exception innerException) : base(message, innerException)
        {
            Email = email;
        }

        public DuplicateEmailException(string email)
        {
            Email = email;
        }

        public DuplicateEmailException()
        {
        }

        public DuplicateEmailException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
