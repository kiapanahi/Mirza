using System;

namespace Mirza.Web.Models
{
    public class AccessKey
    {
        public AccessKey()
        {

        }

        public AccessKey(string key, DateTime expirationDate)
        {
            if (expirationDate <= DateTime.UtcNow)
            {
                throw new ArgumentException("Expiration date must be in the future.", nameof(expirationDate));
            }

            Key = key;
            Expiration = expirationDate;
        }

        public AccessKey(string key) : this(key, DateTime.UtcNow.AddYears(1))
        {

        }

        public int Id { get; set; }
        public string Key { get; set; }
        public AccessKeyState State { get; set; }
        public DateTime Expiration { get; set; }

        public int OwnerId { get; set; }
        public MirzaUser Owner { get; set; }

        public bool IsActive => State == AccessKeyState.Active && Expiration > DateTime.UtcNow;
    }

    public enum AccessKeyState
    {
        Inative = 0,
        Active = 1
    }
}