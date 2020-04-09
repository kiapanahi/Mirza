using System;

namespace Mirza.Web.Services.User
{
    public class UserNotFoundException : Exception
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
    }
}
