using System;

namespace Mirza.Web.Models
{
    public class AccessKey
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public AccessKeyState State { get; set; }
        public DateTime Expriation { get; set; }

        public int OwnerId { get; set; }
        public MirzaUser Owner { get; set; }

        public bool IsActive => State == AccessKeyState.Active && Expriation > DateTime.UtcNow;
    }

    public enum AccessKeyState
    {
        Inative = 0,
        Active = 1
    }
}