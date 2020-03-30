using System;

namespace Mirza.Web.Models
{
    public class WorkLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }

    }
}
