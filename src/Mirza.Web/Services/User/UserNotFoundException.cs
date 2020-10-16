using System;

namespace Mirza.Web.Services.User
{
    public class UserNotFoundException : ArgumentException
    {
        public int UserId { get; set; }
        public UserNotFoundException(int userId, string message) : base(message)
        {
            UserId = userId;
        }

        public UserNotFoundException(int userId, string message, Exception innerException) : base(message, innerException)
        {
            UserId = userId;
        }

        public UserNotFoundException(int userId)
        {
            UserId = userId;
        }

        public UserNotFoundException()
        {
        }

        public UserNotFoundException(string message) : base(message)
        {
        }

        public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
