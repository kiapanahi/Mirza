using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirza.Common;
using Mirza.Web.Models;

namespace Mirza.Web.Dto
{
    public class AccessKeyListItem
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public AccessKeyState State { get; set; }
        public DateTime Expiration { get; set; }
        public string ExpirationPersian => Utils.GetPersianDate(Expiration);
        public bool IsActive => State == AccessKeyState.Active && Expiration > DateTime.UtcNow;

        public AccessKeyListItem(int id, string key, AccessKeyState state, DateTime expirationDate)
        {
            Id = id;
            State = state;
            Expiration = expirationDate;
            var sb = new StringBuilder(key);

            Key = sb.Remove(3, 27)
                .Insert(3, new string('*', 27))
                .ToString();
        }

        public AccessKeyListItem(AccessKey key) : this(key.Id, key.Key, key.State, key.Expiration)
        {
        }
    }
}
